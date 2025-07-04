using Common.DomainEvents;

namespace BookingService.DomainEvents;

public record TempLockCreatedEvent(
    string TempLockId,
    string CourtId,
    string TimeSlotId,
    DateTime PlayDate,
    string LockedBy,
    DateTime ExpiresAt
) : BaseDomainEvent; 