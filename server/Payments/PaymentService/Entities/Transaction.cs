using Common.Base;
using Common.Enums;

namespace PaymentService.Entities;

public class Transaction : BaseEntity
{
    public required string PaymentId { get; set; }
    public required string Type { get; set; } // PAYMENT, REFUND, ADJUSTMENT
    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; }
    public string? ExternalTransactionId { get; set; }
    public string? Gateway { get; set; } // MoMo, VnPay, etc.
    public string? GatewayResponse { get; set; } // JSON
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
    public string? Description { get; set; }
    public string? ErrorCode { get; set; }
    public string? ErrorMessage { get; set; }

    // Navigation property
    public virtual Payment Payment { get; set; } = null!;

    public static Transaction CreatePaymentTransaction(string paymentId, decimal amount, string? gateway = null)
    {
        return new Transaction
        {
            PaymentId = paymentId,
            Type = "PAYMENT",
            Amount = amount,
            Status = PaymentStatus.Pending,
            Gateway = gateway
        };
    }

    public static Transaction CreateRefundTransaction(string paymentId, decimal amount, string? gateway = null)
    {
        return new Transaction
        {
            PaymentId = paymentId,
            Type = "REFUND",
            Amount = amount,
            Status = PaymentStatus.Pending,
            Gateway = gateway
        };
    }

    public void UpdateStatus(PaymentStatus status, string? externalTransactionId = null, string? gatewayResponse = null)
    {
        Status = status;
        ExternalTransactionId = externalTransactionId;
        GatewayResponse = gatewayResponse;
        ProcessedAt = DateTime.UtcNow;
    }

    public void SetError(string errorCode, string errorMessage)
    {
        Status = PaymentStatus.Failed;
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
        ProcessedAt = DateTime.UtcNow;
    }

    public bool IsSuccessful()
    {
        return Status == PaymentStatus.Completed;
    }

    public bool IsRefund()
    {
        return Type == "REFUND";
    }
} 