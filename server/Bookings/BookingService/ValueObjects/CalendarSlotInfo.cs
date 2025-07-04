using Common.Enums;
using BookingService.Entities;

namespace BookingService.ValueObjects;

public record CalendarSlotInfo
{
    public required string SlotId { get; init; }
    public required string CourtId { get; init; }
    public required string CourtName { get; init; }
    public required string TimeSlotId { get; init; }
    public required string TimeSlotDisplay { get; init; }
    public DateTime SlotDateTime { get; init; }
    public SlotStatus Status { get; init; }
    public decimal Price { get; init; }
    public string? BookingId { get; init; }
    public string? UserId { get; init; }
    public string? UserName { get; init; }
    public DateTime? LockExpiry { get; init; }
    public bool IsRecurring { get; init; }
    public string? Notes { get; init; }

    public static CalendarSlotInfo FromCalendarSlot(CalendarSlot slot, string? userName = null)
    {
        return new CalendarSlotInfo
        {
            SlotId = slot.Id,
            CourtId = slot.CourtId,
            CourtName = slot.CourtName ?? "Unknown Court",
            TimeSlotId = slot.TimeSlotId,
            TimeSlotDisplay = slot.TimeSlotDisplay ?? "Unknown Time",
            SlotDateTime = slot.SlotDate,
            Status = slot.Status,
            Price = slot.Price,
            BookingId = slot.BookingId,
            UserId = slot.UserId,
            UserName = userName,
            LockExpiry = slot.LockExpiry,
            IsRecurring = false,
            Notes = slot.Notes
        };
    }

    public bool IsAvailable()
    {
        return Status == SlotStatus.Available;
    }

    public bool IsBooked()
    {
        return Status == SlotStatus.Booked;
    }

    public bool IsTempLocked()
    {
        return Status == SlotStatus.TempLocked;
    }

    public bool IsSelected()
    {
        return Status == SlotStatus.Selected;
    }

    public bool IsUnavailable()
    {
        return Status == SlotStatus.Unavailable;
    }

    public bool IsExpiredLock()
    {
        return Status == SlotStatus.TempLocked 
               && LockExpiry.HasValue 
               && DateTime.UtcNow > LockExpiry.Value;
    }

    public bool CanBeSelectedBy(string userId)
    {
        return Status == SlotStatus.Available 
               || (Status == SlotStatus.Selected && UserId == userId)
               || (IsExpiredLock() && UserId != userId);
    }

    public string GetStatusDisplayText()
    {
        return Status switch
        {
            SlotStatus.Available => "Trống",
            SlotStatus.Booked => "Đã đặt",
            SlotStatus.TempLocked => "Tạm khóa",
            SlotStatus.Selected => "Đang chọn",
            SlotStatus.Unavailable => "Không khả dụng",
            _ => "Unknown"
        };
    }

    public string GetCssClass()
    {
        return Status switch
        {
            SlotStatus.Available => "slot-available",
            SlotStatus.Booked => "slot-booked",
            SlotStatus.TempLocked => IsExpiredLock() ? "slot-expired" : "slot-locked",
            SlotStatus.Selected => "slot-selected",
            SlotStatus.Unavailable => "slot-unavailable",
            _ => "slot-unknown"
        };
    }
} 