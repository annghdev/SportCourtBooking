using Common.DomainEvents;

namespace BookingService.DomainEvents;

public record BookingCompletedEvent(
    string BookingId,
    string UserId,
    string FacilityId
) : BaseDomainEvent; 