namespace Common.Enums;

public enum BookingStatus
{
    Pending = 1,        // Chờ xử lý
    Confirmed = 2,      // Đã xác nhận  
    InProgress = 3,     // Đang diễn ra
    Completed = 4,      // Hoàn thành
    Cancelled = 5,      // Đã hủy
    NoShow = 6          // Không có mặt
} 