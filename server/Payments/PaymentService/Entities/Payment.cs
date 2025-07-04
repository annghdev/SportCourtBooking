using Common.Base;
using Common.Enums;
using PaymentService.DomainEvents;

namespace PaymentService.Entities;

public class Payment : AggregateRoot
{
    public required string BookingId { get; set; }
    public required string UserId { get; set; }
    public PaymentMethod Method { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public decimal Amount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal FinalAmount { get; set; }
    public string? Description { get; set; }
    public string? ExternalTransactionId { get; set; } // ID từ gateway thanh toán
    public string? PaymentGatewayResponse { get; set; } // JSON response
    public DateTime? PaidAt { get; set; }
    public DateTime? RefundedAt { get; set; }
    public decimal RefundAmount { get; set; }
    public string? FailureReason { get; set; }

    // Navigation properties
    public virtual ICollection<Transaction> Transactions { get; set; } = [];

    public static Payment Create(string bookingId, string userId, PaymentMethod method, decimal amount, string? description = null)
    {
        var payment = new Payment
        {
            BookingId = bookingId,
            UserId = userId,
            Method = method,
            Amount = amount,
            FinalAmount = amount,
            Description = description
        };

        payment.AddDomainEvent(new PaymentInitiatedEvent(payment.Id, bookingId, userId, method, amount));
        return payment;
    }

    public void ApplyDiscount(decimal discountAmount, string updatedBy)
    {
        DiscountAmount = discountAmount;
        FinalAmount = Amount - DiscountAmount;
        UpdatedBy = updatedBy;
        UpdatedDate = DateTimeOffset.UtcNow;
    }

    public void ProcessPayment(string externalTransactionId, string gatewayResponse, string processedBy)
    {
        Status = PaymentStatus.Processing;
        ExternalTransactionId = externalTransactionId;
        PaymentGatewayResponse = gatewayResponse;
        UpdatedBy = processedBy;
        UpdatedDate = DateTimeOffset.UtcNow;
    }

    public void CompletePayment(string completedBy)
    {
        Status = PaymentStatus.Completed;
        PaidAt = DateTime.UtcNow;
        UpdatedBy = completedBy;
        UpdatedDate = DateTimeOffset.UtcNow;

        AddDomainEvent(new PaymentCompletedEvent(Id, BookingId, UserId, FinalAmount, Method));
    }

    public void FailPayment(string reason, string failedBy)
    {
        Status = PaymentStatus.Failed;
        FailureReason = reason;
        UpdatedBy = failedBy;
        UpdatedDate = DateTimeOffset.UtcNow;

        AddDomainEvent(new PaymentFailedEvent(Id, BookingId, UserId, reason));
    }

    public void CancelPayment(string cancelledBy)
    {
        Status = PaymentStatus.Cancelled;
        UpdatedBy = cancelledBy;
        UpdatedDate = DateTimeOffset.UtcNow;
    }

    public void RefundPayment(decimal refundAmount, string refundedBy, string? reason = null)
    {
        if (Status != PaymentStatus.Completed)
            throw new InvalidOperationException("Can only refund completed payments");

        if (refundAmount > FinalAmount)
            throw new ArgumentException("Refund amount cannot exceed payment amount");

        Status = PaymentStatus.Refunded;
        RefundAmount = refundAmount;
        RefundedAt = DateTime.UtcNow;
        UpdatedBy = refundedBy;
        UpdatedDate = DateTimeOffset.UtcNow;
        Description = reason;

        AddDomainEvent(new PaymentRefundedEvent(Id, BookingId, UserId, refundAmount, reason));
    }

    public bool IsOnlinePayment()
    {
        return Method is PaymentMethod.MoMo or PaymentMethod.VnPay or PaymentMethod.BankTransfer;
    }

    public bool CanBeRefunded()
    {
        return Status == PaymentStatus.Completed && RefundAmount == 0;
    }
} 