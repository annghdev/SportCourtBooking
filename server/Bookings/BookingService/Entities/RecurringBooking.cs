using Common.Base;
using Common.Enums;
using BookingService.DomainEvents;

namespace BookingService.Entities;

public class RecurringBooking : AggregateRoot
{
    public required string UserId { get; set; }
    public required string FacilityId { get; set; }
    public required string CourtId { get; set; }
    public required string TimeSlotId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; } // null = vô thời hạn
    public required string DaysOfWeek { get; set; } // JSON array of DayOfWeek numbers
    public decimal PricePerSession { get; set; }
    public BookingStatus Status { get; set; } = BookingStatus.Confirmed;
    public bool IsActive { get; set; } = true;
    public string? Notes { get; set; }
    
    // Tracking
    public int TotalSessions { get; set; }
    public int CompletedSessions { get; set; }
    public int CancelledSessions { get; set; }

    // Calendar generation optimization
    public DateTime? LastGeneratedDate { get; set; }
    public bool AutoGenerateInstances { get; set; } = true;

    // Navigation properties
    public virtual ICollection<RecurringBookingInstance> Instances { get; set; } = [];

    public static RecurringBooking Create(string userId, string facilityId, string courtId, string timeSlotId,
        DateTime startDate, DateTime? endDate, IEnumerable<DayOfWeek> daysOfWeek, decimal pricePerSession, string? notes = null)
    {
        var recurringBooking = new RecurringBooking
        {
            UserId = userId,
            FacilityId = facilityId,
            CourtId = courtId,
            TimeSlotId = timeSlotId,
            StartDate = startDate,
            EndDate = endDate,
            DaysOfWeek = System.Text.Json.JsonSerializer.Serialize(daysOfWeek.Cast<int>()),
            PricePerSession = pricePerSession,
            Notes = notes
        };

        recurringBooking.AddDomainEvent(new RecurringBookingCreatedEvent(
            recurringBooking.Id, userId, facilityId, courtId, timeSlotId, startDate, endDate, daysOfWeek));

        return recurringBooking;
    }

    public IEnumerable<DayOfWeek> GetDaysOfWeek()
    {
        var dayNumbers = System.Text.Json.JsonSerializer.Deserialize<int[]>(DaysOfWeek) ?? [];
        return dayNumbers.Cast<DayOfWeek>();
    }

    public void UpdateSchedule(DateTime startDate, DateTime? endDate, IEnumerable<DayOfWeek> daysOfWeek, string updatedBy)
    {
        StartDate = startDate;
        EndDate = endDate;
        DaysOfWeek = System.Text.Json.JsonSerializer.Serialize(daysOfWeek.Cast<int>());
        UpdatedBy = updatedBy;
        UpdatedDate = DateTimeOffset.UtcNow;

        // Reset generation tracking to regenerate instances
        LastGeneratedDate = null;
    }

    public void UpdatePrice(decimal newPrice, string updatedBy)
    {
        PricePerSession = newPrice;
        UpdatedBy = updatedBy;
        UpdatedDate = DateTimeOffset.UtcNow;
    }

    public void Cancel(string cancelledBy, string? reason = null)
    {
        Status = BookingStatus.Cancelled;
        IsActive = false;
        UpdatedBy = cancelledBy;
        UpdatedDate = DateTimeOffset.UtcNow;
        Notes = reason;

        // Cancel all future instances
        var futureInstances = Instances.Where(i => i.PlayDate >= DateTime.Today && i.Status == BookingStatus.Confirmed);
        foreach (var instance in futureInstances)
        {
            instance.Cancel(reason ?? "Recurring booking cancelled");
        }
    }

    public void IncrementCompletedSessions()
    {
        CompletedSessions++;
    }

    public void IncrementCancelledSessions()
    {
        CancelledSessions++;
    }

    public List<DateTime> GenerateBookingDates(DateTime? fromDate = null, DateTime? toDate = null)
    {
        var dates = new List<DateTime>();
        var daysOfWeek = GetDaysOfWeek().ToHashSet();
        
        var startDate = fromDate ?? StartDate.Date;
        var endDate = toDate ?? EndDate?.Date ?? DateTime.Today.AddMonths(6); // Default 6 months

        var currentDate = startDate;
        
        while (currentDate <= endDate)
        {
            if (daysOfWeek.Contains(currentDate.DayOfWeek))
            {
                dates.Add(currentDate);
            }
            currentDate = currentDate.AddDays(1);
        }

        return dates;
    }

    public void GenerateInstances(DateTime upToDate)
    {
        if (!AutoGenerateInstances || !IsActive) return;

        var lastGenerated = LastGeneratedDate ?? StartDate.AddDays(-1);
        var datesToGenerate = GenerateBookingDates(lastGenerated.AddDays(1), upToDate);

        foreach (var date in datesToGenerate)
        {
            // Check if instance already exists
            if (!Instances.Any(i => i.PlayDate.Date == date.Date))
            {
                var instance = RecurringBookingInstance.Create(Id, date, PricePerSession);
                Instances.Add(instance);
                TotalSessions++;
            }
        }

        LastGeneratedDate = upToDate;
        UpdatedDate = DateTimeOffset.UtcNow;
    }

    public void CancelInstance(DateTime playDate, string reason, string cancelledBy)
    {
        var instance = Instances.FirstOrDefault(i => i.PlayDate.Date == playDate.Date);
        if (instance != null && instance.Status == BookingStatus.Confirmed)
        {
            instance.Cancel(reason);
            CancelledSessions++;
            UpdatedBy = cancelledBy;
            UpdatedDate = DateTimeOffset.UtcNow;
        }
    }

    public void CompleteInstance(DateTime playDate)
    {
        var instance = Instances.FirstOrDefault(i => i.PlayDate.Date == playDate.Date);
        if (instance != null && instance.Status == BookingStatus.Confirmed)
        {
            instance.Complete();
            CompletedSessions++;
            UpdatedDate = DateTimeOffset.UtcNow;
        }
    }

    public List<RecurringBookingInstance> GetUpcomingInstances(int daysAhead = 30)
    {
        var cutoffDate = DateTime.Today.AddDays(daysAhead);
        return Instances
            .Where(i => i.PlayDate >= DateTime.Today && i.PlayDate <= cutoffDate)
            .OrderBy(i => i.PlayDate)
            .ToList();
    }

    public List<RecurringBookingInstance> GetInstancesForDateRange(DateTime fromDate, DateTime toDate)
    {
        return Instances
            .Where(i => i.PlayDate.Date >= fromDate.Date && i.PlayDate.Date <= toDate.Date)
            .OrderBy(i => i.PlayDate)
            .ToList();
    }

    public bool ConflictsWithDate(DateTime date)
    {
        return IsActive 
               && Status == BookingStatus.Confirmed
               && GetDaysOfWeek().Contains(date.DayOfWeek)
               && date.Date >= StartDate.Date
               && (!EndDate.HasValue || date.Date <= EndDate.Value.Date);
    }

    public decimal GetTotalRevenue()
    {
        return Instances.Where(i => i.Status == BookingStatus.Completed).Sum(i => i.Price);
    }
} 