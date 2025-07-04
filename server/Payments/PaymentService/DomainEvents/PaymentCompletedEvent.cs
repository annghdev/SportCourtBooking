using Common.DomainEvents;
using Common.Enums;

namespace PaymentService.DomainEvents;

public record PaymentCompletedEvent(
    string PaymentId,
    string BookingId,
    string UserId,
    decimal Amount,
    PaymentMethod Method
) : BaseDomainEvent; 