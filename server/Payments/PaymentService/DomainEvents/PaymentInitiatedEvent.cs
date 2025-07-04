using Common.DomainEvents;
using Common.Enums;

namespace PaymentService.DomainEvents;

public record PaymentInitiatedEvent(
    string PaymentId,
    string BookingId,
    string UserId,
    PaymentMethod Method,
    decimal Amount
) : BaseDomainEvent; 