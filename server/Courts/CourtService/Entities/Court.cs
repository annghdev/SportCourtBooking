using Common.Base;
using Common.Enums;
using CourtService.DomainEvents;

namespace CourtService.Entities;

public class Court : AggregateRoot
{
    public required string Name { get; set; }
    public required string FacilityId { get; set; }
    public CourtType Type { get; set; }
    public decimal BasePrice { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public int Capacity { get; set; } = 10; // Số người tối đa

    // Navigation properties
    public virtual Facility Facility { get; set; } = null!;
    public virtual ICollection<TimeSlot> TimeSlots { get; set; } = [];
    public virtual ICollection<CourtPricing> CourtPricings { get; set; } = [];
    public virtual ICollection<CourtOperatingHours> OperatingHours { get; set; } = [];
    public virtual ICollection<CourtMaintenance> MaintenanceSchedules { get; set; } = [];
    public virtual ICollection<CourtAvailability> BlockedAvailabilities { get; set; } = [];

    public static Court Create(string name, string facilityId, CourtType type, decimal basePrice, string? description = null)
    {
        var court = new Court
        {
            Name = name,
            FacilityId = facilityId,
            Type = type,
            BasePrice = basePrice,
            Description = description
        };

        court.AddDomainEvent(new CourtCreatedEvent(court.Id, court.Name, court.FacilityId, court.Type));
        return court;
    }

    public void UpdateInfo(string name, string? description, int capacity)
    {
        Name = name;
        Description = description;
        Capacity = capacity;
        UpdatedDate = DateTimeOffset.UtcNow;
    }

    public void UpdateBasePrice(decimal newPrice, string updatedBy)
    {
        BasePrice = newPrice;
        UpdatedBy = updatedBy;
        UpdatedDate = DateTimeOffset.UtcNow;
    }

    public void SetUnavailable(string updatedBy, string? reason = null)
    {
        IsActive = false;
        UpdatedBy = updatedBy;
        UpdatedDate = DateTimeOffset.UtcNow;

        AddDomainEvent(new CourtUnavailableEvent(Id, reason));
    }

    public void SetAvailable(string updatedBy)
    {
        IsActive = true;
        UpdatedBy = updatedBy;
        UpdatedDate = DateTimeOffset.UtcNow;

        AddDomainEvent(new CourtAvailableEvent(Id));
    }

    public void SetOperatingHours(DayOfWeek dayOfWeek, TimeOnly openTime, TimeOnly closeTime, string updatedBy)
    {
        var existingHours = OperatingHours.FirstOrDefault(h => h.DayOfWeek == dayOfWeek);
        if (existingHours != null)
        {
            existingHours.UpdateHours(openTime, closeTime);
        }
        else
        {
            var newHours = CourtOperatingHours.Create(Id, dayOfWeek, openTime, closeTime);
            OperatingHours.Add(newHours);
        }
        
        UpdatedBy = updatedBy;
        UpdatedDate = DateTimeOffset.UtcNow;
    }

    public void CloseForDay(DayOfWeek dayOfWeek, string updatedBy, string? reason = null)
    {
        var operatingHours = OperatingHours.FirstOrDefault(h => h.DayOfWeek == dayOfWeek);
        if (operatingHours != null)
        {
            operatingHours.CloseForDay(reason);
        }
        
        UpdatedBy = updatedBy;
        UpdatedDate = DateTimeOffset.UtcNow;
    }

    public void OpenForDay(DayOfWeek dayOfWeek, string updatedBy)
    {
        var operatingHours = OperatingHours.FirstOrDefault(h => h.DayOfWeek == dayOfWeek);
        if (operatingHours != null)
        {
            operatingHours.OpenForDay();
        }
        
        UpdatedBy = updatedBy;
        UpdatedDate = DateTimeOffset.UtcNow;
    }

    public void SetPricing(string timeSlotId, decimal price, string updatedBy, DayOfWeek? dayOfWeek = null, DateTime? effectiveFrom = null, DateTime? effectiveTo = null)
    {
        var existingPricing = CourtPricings.FirstOrDefault(p => p.TimeSlotId == timeSlotId && p.DayOfWeek == dayOfWeek);

        if (existingPricing != null)
        {
            existingPricing.UpdatePrice(price);
        }
        else
        {
            var newPricing = CourtPricing.Create(Id, timeSlotId, price, dayOfWeek, effectiveFrom, effectiveTo);
            CourtPricings.Add(newPricing);
        }

        UpdatedBy = updatedBy;
        UpdatedDate = DateTimeOffset.UtcNow;
    }

    public void RemovePricing(string pricingId, string updatedBy)
    {
        var pricingToRemove = CourtPricings.FirstOrDefault(p => p.Id == pricingId);
        if (pricingToRemove != null)
        {
            // Instead of removing, we deactivate it to keep history.
            pricingToRemove.IsActive = false;
        }

        UpdatedBy = updatedBy;
        UpdatedDate = DateTimeOffset.UtcNow;
    }

    public CourtMaintenance ScheduleMaintenance(string title, DateTime startTime, DateTime endTime, 
        MaintenanceType type, string scheduledBy, string? description = null, string? assignedTo = null)
    {
        // Check for conflicts with existing maintenance
        var conflictingMaintenance = MaintenanceSchedules
            .Where(m => m.ConflictsWith(startTime, endTime))
            .FirstOrDefault();

        if (conflictingMaintenance != null)
        {
            throw new InvalidOperationException($"Maintenance conflicts with existing maintenance: {conflictingMaintenance.Title}");
        }

        var maintenance = CourtMaintenance.Schedule(Id, title, startTime, endTime, type, description, assignedTo);
        MaintenanceSchedules.Add(maintenance);
        
        UpdatedBy = scheduledBy;
        UpdatedDate = DateTimeOffset.UtcNow;

        return maintenance;
    }

    public void StartMaintenance(string maintenanceId, string startedBy)
    {
        var maintenance = MaintenanceSchedules.FirstOrDefault(m => m.Id == maintenanceId);
        if (maintenance == null) throw new KeyNotFoundException("Maintenance schedule not found.");

        maintenance.Start();
        UpdatedBy = startedBy;
        UpdatedDate = DateTimeOffset.UtcNow;
    }

    public void CompleteMaintenance(string maintenanceId, string completedBy, decimal? actualCost = null, string? notes = null)
    {
        var maintenance = MaintenanceSchedules.FirstOrDefault(m => m.Id == maintenanceId);
        if (maintenance == null) throw new KeyNotFoundException("Maintenance schedule not found.");
        
        maintenance.Complete(actualCost, notes);
        UpdatedBy = completedBy;
        UpdatedDate = DateTimeOffset.UtcNow;
    }

    public void CancelMaintenance(string maintenanceId, string cancelledBy, string? reason = null)
    {
        var maintenance = MaintenanceSchedules.FirstOrDefault(m => m.Id == maintenanceId);
        if (maintenance == null) throw new KeyNotFoundException("Maintenance schedule not found.");
        
        maintenance.Cancel(reason);
        UpdatedBy = cancelledBy;
        UpdatedDate = DateTimeOffset.UtcNow;
    }

    public void RescheduleMaintenance(string maintenanceId, DateTime newStartTime, DateTime newEndTime, string rescheduledBy, string? reason = null)
    {
        var maintenance = MaintenanceSchedules.FirstOrDefault(m => m.Id == maintenanceId);
        if (maintenance == null) throw new KeyNotFoundException("Maintenance schedule not found.");

        // Optional: Add conflict check here as well before rescheduling
        var conflictingMaintenance = MaintenanceSchedules
            .Where(m => m.Id != maintenanceId && m.ConflictsWith(newStartTime, newEndTime))
            .FirstOrDefault();

        if (conflictingMaintenance != null)
        {
            throw new InvalidOperationException($"Rescheduling conflicts with existing maintenance: {conflictingMaintenance.Title}");
        }

        maintenance.Reschedule(newStartTime, newEndTime, reason);
        UpdatedBy = rescheduledBy;
        UpdatedDate = DateTimeOffset.UtcNow;
    }

    public void BlockAvailability(string timeSlotId, DateTime fromDate, string blockedBy, DateTime? toDate = null, string? reason = null)
    {
        // Optional: Check for conflicts with existing bookings if this service has access to that info.
        // For now, we just add the block.

        var existingBlock = BlockedAvailabilities.FirstOrDefault(b => b.TimeSlotId == timeSlotId && b.IsBlockedOnDate(fromDate));
        if (existingBlock != null)
        {
            existingBlock.UpdateBlockPeriod(fromDate, toDate);
        }
        else
        {
            var block = CourtAvailability.CreateBlock(Id, timeSlotId, fromDate, toDate, reason);
            BlockedAvailabilities.Add(block);
        }

        UpdatedBy = blockedBy;
        UpdatedDate = DateTimeOffset.UtcNow;
    }

    public void UnblockAvailability(string availabilityId, string unblockedBy)
    {
        var block = BlockedAvailabilities.FirstOrDefault(b => b.Id == availabilityId);
        if (block != null)
        {
            block.Unblock();
        }

        UpdatedBy = unblockedBy;
        UpdatedDate = DateTimeOffset.UtcNow;
    }

    public bool IsOperatingOnDateTime(DateTime dateTime)
    {
        if (!IsActive) return false;

        var dayOperatingHours = OperatingHours.FirstOrDefault(h => h.DayOfWeek == dateTime.DayOfWeek);
        if (dayOperatingHours == null || !dayOperatingHours.IsActive || dayOperatingHours.IsClosed)
            return false;

        var timeOnly = TimeOnly.FromDateTime(dateTime);
        return dayOperatingHours.IsWithinOperatingHours(timeOnly);
    }

    public bool HasMaintenanceOnDateTime(DateTime dateTime)
    {
        return MaintenanceSchedules.Any(m => m.ConflictsWith(dateTime, dateTime.AddMinutes(1)));
    }

    public bool IsAvailableForBooking(DateTime dateTime)
    {
        return IsOperatingOnDateTime(dateTime) && !HasMaintenanceOnDateTime(dateTime);
    }

    public List<CourtOperatingHours> GetWeeklyOperatingHours()
    {
        return OperatingHours.Where(h => h.IsActive).OrderBy(h => h.DayOfWeek).ToList();
    }

    public List<CourtMaintenance> GetUpcomingMaintenance(int daysAhead = 30)
    {
        var cutoffDate = DateTime.Now.AddDays(daysAhead);
        return MaintenanceSchedules
            .Where(m => m.StartTime <= cutoffDate && m.Status != MaintenanceStatus.Cancelled)
            .OrderBy(m => m.StartTime)
            .ToList();
    }
}
