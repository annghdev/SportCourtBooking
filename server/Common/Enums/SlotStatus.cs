namespace Common.Enums;

public enum SlotStatus
{
    Available = 1,      // Trống
    Unavailable = 2,    // Không khả dụng
    TempLocked = 3,     // Tạm khóa
    Selected = 4,       // Đang chọn
    Booked = 5          // Đã đặt
} 