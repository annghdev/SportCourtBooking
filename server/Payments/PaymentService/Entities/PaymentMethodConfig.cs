using Common.Base;
using Common.Enums;

namespace PaymentService.Entities;

public class PaymentMethodConfig : AggregateRoot
{
    public PaymentMethod Method { get; set; }
    public bool IsEnabled { get; set; } = true;
    public string? DisplayName { get; set; }
    public string? Description { get; set; }
    public decimal MinAmount { get; set; }
    public decimal MaxAmount { get; set; }
    public int TimeoutMinutes { get; set; } = 15; // Default 15 phút
    public string? ConfigJson { get; set; } // Cấu hình riêng cho từng gateway
    public int SortOrder { get; set; }

    public static PaymentMethodConfig Create(PaymentMethod method, string displayName, decimal minAmount = 0, decimal maxAmount = 10000000)
    {
        return new PaymentMethodConfig
        {
            Method = method,
            DisplayName = displayName,
            MinAmount = minAmount,
            MaxAmount = maxAmount,
            SortOrder = (int)method
        };
    }

    public void UpdateConfig(string displayName, string? description, decimal minAmount, decimal maxAmount, int timeoutMinutes, string updatedBy)
    {
        DisplayName = displayName;
        Description = description;
        MinAmount = minAmount;
        MaxAmount = maxAmount;
        TimeoutMinutes = timeoutMinutes;
        UpdatedBy = updatedBy;
        UpdatedDate = DateTimeOffset.UtcNow;
    }

    public void Enable(string updatedBy)
    {
        IsEnabled = true;
        UpdatedBy = updatedBy;
        UpdatedDate = DateTimeOffset.UtcNow;
    }

    public void Disable(string updatedBy)
    {
        IsEnabled = false;
        UpdatedBy = updatedBy;
        UpdatedDate = DateTimeOffset.UtcNow;
    }

    public bool IsValidAmount(decimal amount)
    {
        return amount >= MinAmount && amount <= MaxAmount;
    }

    public bool IsOnlineMethod()
    {
        return Method is PaymentMethod.MoMo or PaymentMethod.VnPay or PaymentMethod.BankTransfer;
    }
} 