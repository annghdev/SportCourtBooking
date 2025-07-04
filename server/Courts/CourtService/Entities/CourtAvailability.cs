using Common.Base;

namespace CourtService.Entities;

public class CourtAvailability : BaseEntity
{
    public required string CourtId { get; set; }
    public required string TimeSlotId { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime? ToDate { get; set; } // null = vô thời hạn
    public bool IsBlocked { get; set; } = true; // true = khóa, false = mở
    public string? Reason { get; set; }

    // Navigation properties
    public virtual Court Court { get; set; } = null!;
    public virtual TimeSlot TimeSlot { get; set; } = null!;

    public static CourtAvailability CreateBlock(string courtId, string timeSlotId, DateTime fromDate, 
        DateTime? toDate = null, string? reason = null)
    {
        return new CourtAvailability
        {
            CourtId = courtId,
            TimeSlotId = timeSlotId,
            FromDate = fromDate,
            ToDate = toDate,
            IsBlocked = true,
            Reason = reason
        };
    }

    public void UpdateBlockPeriod(DateTime fromDate, DateTime? toDate)
    {
        FromDate = fromDate;
        ToDate = toDate;
    }

    public void Unblock()
    {
        IsBlocked = false;
    }

    public bool IsBlockedOnDate(DateTime date)
    {
        if (!IsBlocked) return false;

        if (date < FromDate.Date) return false;

        if (ToDate.HasValue && date > ToDate.Value.Date) return false;

        return true;
    }
} 