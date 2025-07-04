using Common.DomainEvents;
using Common.Enums;

namespace BookingService.DomainEvents;

public record BookingCreatedEvent(
    string BookingId,
    string UserId,
    string FacilityId,
    BookingType Type
) : BaseDomainEvent; 