using Common.DomainEvents;

namespace BookingService.DomainEvents;

public record BookingCancelledEvent(
    string BookingId,
    string UserId,
    string? Reason
) : BaseDomainEvent; 