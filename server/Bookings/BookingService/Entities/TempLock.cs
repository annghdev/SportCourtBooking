using Common.Base;
using BookingService.DomainEvents;

namespace BookingService.Entities;

//Tạm khóa slot đặt sân khi user bấm chọn / admin tạm khóa khung giờ
public class TempLock : BaseEntity
{
    public required string CourtId { get; set; }
    public required string TimeSlotId { get; set; }
    public DateTime PlayDate { get; set; }
    public required string LockedBy { get; set; } // UserId hoặc system
    public DateTime ExpiresAt { get; set; }
    public bool IsActive { get; set; } = true;
    public string? BookingId { get; set; } // Nếu lock để process booking
    public string? Reason { get; set; }

    public static TempLock CreateUserLock(string courtId, string timeSlotId, DateTime playDate, string userId, int lockDurationMinutes = 10)
    {
        var tempLock = new TempLock
        {
            CourtId = courtId,
            TimeSlotId = timeSlotId,
            PlayDate = playDate,
            LockedBy = userId,
            ExpiresAt = DateTime.UtcNow.AddMinutes(lockDurationMinutes),
            Reason = "User selection"
        };

        tempLock.AddDomainEvent(new TempLockCreatedEvent(tempLock.Id, courtId, timeSlotId, playDate, userId, tempLock.ExpiresAt));
        return tempLock;
    }

    public static TempLock CreateAdminLock(string courtId, string timeSlotId, DateTime playDate, string adminId, DateTime expiresAt, string? reason = null)
    {
        var tempLock = new TempLock
        {
            CourtId = courtId,
            TimeSlotId = timeSlotId,
            PlayDate = playDate,
            LockedBy = adminId,
            ExpiresAt = expiresAt,
            Reason = reason ?? "Admin lock"
        };

        tempLock.AddDomainEvent(new TempLockCreatedEvent(tempLock.Id, courtId, timeSlotId, playDate, adminId, expiresAt));
        return tempLock;
    }

    public static TempLock CreateBookingLock(string courtId, string timeSlotId, DateTime playDate, string bookingId, int lockDurationMinutes = 15)
    {
        var tempLock = new TempLock
        {
            CourtId = courtId,
            TimeSlotId = timeSlotId,
            PlayDate = playDate,
            LockedBy = "SYSTEM",
            BookingId = bookingId,
            ExpiresAt = DateTime.UtcNow.AddMinutes(lockDurationMinutes),
            Reason = "Booking processing"
        };

        tempLock.AddDomainEvent(new TempLockCreatedEvent(tempLock.Id, courtId, timeSlotId, playDate, "SYSTEM", tempLock.ExpiresAt));
        return tempLock;
    }

    public void ExtendLock(int additionalMinutes)
    {
        ExpiresAt = ExpiresAt.AddMinutes(additionalMinutes);
    }

    public void ReleaseLock()
    {
        IsActive = false;
        AddDomainEvent(new TempLockExpiredEvent(Id, CourtId, TimeSlotId, PlayDate));
    }

    public bool IsExpired()
    {
        return DateTime.UtcNow > ExpiresAt || !IsActive;
    }

    public bool IsValidForUser(string userId)
    {
        return LockedBy == userId && !IsExpired();
    }
}
