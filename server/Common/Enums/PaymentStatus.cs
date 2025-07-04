namespace Common.Enums;

public enum PaymentStatus
{
    Pending = 1,        // Chờ thanh toán
    Processing = 2,     // Đang xử lý
    Completed = 3,      // Đã thanh toán
    Failed = 4,         // Thất bại
    Cancelled = 5,      // Đã hủy
    Refunded = 6        // Đã hoàn tiền
} 