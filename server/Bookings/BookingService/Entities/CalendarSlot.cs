using Common.Base;
using Common.Enums;
using BookingService.DomainEvents;

namespace BookingService.Entities;

public class CalendarSlot : BaseEntity
{
    public required string CourtId { get; set; }
    public required string TimeSlotId { get; set; }
    public DateTime SlotDate { get; set; }
    public SlotStatus Status { get; set; } = SlotStatus.Available;
    public decimal Price { get; set; }
    public string? BookingId { get; set; }
    public string? TempLockId { get; set; }
    public string? UserId { get; set; } // User đang chọn hoặc đã book
    public DateTime? LockExpiry { get; set; }
    public string? Notes { get; set; }

    // Denormalized data for quick access
    public string? CourtName { get; set; }
    public string? TimeSlotDisplay { get; set; }

    public static CalendarSlot Create(string courtId, string timeSlotId, DateTime slotDate, decimal price)
    {
        return new CalendarSlot
        {
            CourtId = courtId,
            TimeSlotId = timeSlotId,
            SlotDate = slotDate,
            Price = price
        };
    }

    public void MarkAsBooked(string bookingId, string userId)
    {
        var oldStatus = Status;
        Status = SlotStatus.Booked;
        BookingId = bookingId;
        UserId = userId;
        TempLockId = null;
        LockExpiry = null;

        AddDomainEvent(new CalendarSlotStatusChangedEvent(Id, CourtId, TimeSlotId, SlotDate, oldStatus, Status, userId, bookingId, "Booking confirmed"));
    }

    public void MarkAsTempLocked(string tempLockId, string userId, DateTime lockExpiry)
    {
        var oldStatus = Status;
        Status = SlotStatus.TempLocked;
        TempLockId = tempLockId;
        UserId = userId;
        LockExpiry = lockExpiry;
        BookingId = null;

        AddDomainEvent(new CalendarSlotStatusChangedEvent(Id, CourtId, TimeSlotId, SlotDate, oldStatus, Status, userId, null, "Temporarily locked"));
    }

    public void MarkAsSelected(string userId)
    {
        var oldStatus = Status;
        Status = SlotStatus.Selected;
        UserId = userId;
        BookingId = null;
        TempLockId = null;

        AddDomainEvent(new CalendarSlotStatusChangedEvent(Id, CourtId, TimeSlotId, SlotDate, oldStatus, Status, userId, null, "User selection"));
    }

    public void MarkAsUnavailable(string? reason = null)
    {
        var oldStatus = Status;
        Status = SlotStatus.Unavailable;
        Notes = reason;
        BookingId = null;
        TempLockId = null;
        UserId = null;
        LockExpiry = null;

        AddDomainEvent(new CalendarSlotStatusChangedEvent(Id, CourtId, TimeSlotId, SlotDate, oldStatus, Status, null, null, reason));
    }

    public void MarkAsAvailable()
    {
        var oldStatus = Status;
        Status = SlotStatus.Available;
        BookingId = null;
        TempLockId = null;
        UserId = null;
        LockExpiry = null;
        Notes = null;

        AddDomainEvent(new CalendarSlotStatusChangedEvent(Id, CourtId, TimeSlotId, SlotDate, oldStatus, Status, null, null, "Reset to available"));
    }

    public void UpdatePrice(decimal newPrice)
    {
        Price = newPrice;
    }

    public void UpdateDisplayInfo(string courtName, string timeSlotDisplay)
    {
        CourtName = courtName;
        TimeSlotDisplay = timeSlotDisplay;
    }

    public bool IsExpiredLock()
    {
        return Status == SlotStatus.TempLocked 
               && LockExpiry.HasValue 
               && DateTime.UtcNow > LockExpiry.Value;
    }

    public bool IsAvailableForUser(string userId)
    {
        return Status == SlotStatus.Available 
               || (Status == SlotStatus.Selected && UserId == userId)
               || (Status == SlotStatus.TempLocked && UserId == userId && !IsExpiredLock());
    }

    public bool CanBeBookedBy(string userId)
    {
        return Status == SlotStatus.Available 
               || (Status == SlotStatus.Selected && UserId == userId)
               || (Status == SlotStatus.TempLocked && UserId == userId && !IsExpiredLock());
    }

    public DateTime GetSlotDateTime(TimeOnly startTime)
    {
        return SlotDate.Date.Add(startTime.ToTimeSpan());
    }

    public string GetUniqueKey()
    {
        return $"{CourtId}_{TimeSlotId}_{SlotDate:yyyy-MM-dd}";
    }

    public void ExpireLock()
    {
        if (Status == SlotStatus.TempLocked && IsExpiredLock())
        {
            MarkAsAvailable();
        }
    }

    public void SyncWithBooking(string bookingId, string userId, BookingStatus bookingStatus)
    {
        switch (bookingStatus)
        {
            case BookingStatus.Confirmed:
                MarkAsBooked(bookingId, userId);
                break;
            case BookingStatus.Cancelled:
                MarkAsAvailable();
                break;
            case BookingStatus.Pending:
                if (Status != SlotStatus.TempLocked)
                    MarkAsTempLocked(bookingId, userId, DateTime.UtcNow.AddMinutes(15));
                break;
        }
    }

    public void SyncWithTempLock(string tempLockId, string userId, DateTime expiry, bool isActive)
    {
        if (isActive && DateTime.UtcNow < expiry)
        {
            MarkAsTempLocked(tempLockId, userId, expiry);
        }
        else
        {
            MarkAsAvailable();
        }
    }
} 