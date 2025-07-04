using Common.DomainEvents;

namespace PaymentService.DomainEvents;

public record PaymentRefundedEvent(
    string PaymentId,
    string BookingId,
    string UserId,
    decimal RefundAmount,
    string? Reason
) : BaseDomainEvent; 