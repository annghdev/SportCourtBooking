using Common.DomainEvents;

namespace BookingService.DomainEvents;

public record TempLockExpiredEvent(
    string TempLockId,
    string CourtId,
    string TimeSlotId,
    DateTime PlayDate
) : BaseDomainEvent; 