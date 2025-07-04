using Common.DomainEvents;

namespace BookingService.DomainEvents;

public record BookingConfirmedEvent(
    string BookingId,
    string UserId,
    string FacilityId,
    decimal FinalAmount
) : BaseDomainEvent; 