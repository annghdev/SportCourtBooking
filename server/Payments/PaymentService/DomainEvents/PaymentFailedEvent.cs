using Common.DomainEvents;

namespace PaymentService.DomainEvents;

public record PaymentFailedEvent(
    string PaymentId,
    string BookingId,
    string UserId,
    string Reason
) : BaseDomainEvent; 