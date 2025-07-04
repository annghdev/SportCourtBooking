namespace Common.Enums;

public enum PermissionType
{
    // User Management
    ViewUsers = 1001,
    CreateUsers = 1002,
    UpdateUsers = 1003,
    DeleteUsers = 1004,
    ChangeUserRoles = 1005,

    // Facility Management
    ViewFacilities = 2001,
    CreateFacilities = 2002,
    UpdateFacilities = 2003,
    DeleteFacilities = 2004,
    ManageFacilityOperatingHours = 2005,

    // Court Management
    ViewCourts = 3001,
    CreateCourts = 3002,
    UpdateCourts = 3003,
    DeleteCourts = 3004,
    ManageCourtAvailability = 3005,
    UpdateCourtPricing = 3006,
    ManageCourtOperatingHours = 3007,

    // Booking Management
    ViewAllBookings = 4001,
    ViewOwnBookings = 4002,
    CreateBookings = 4003,
    UpdateBookings = 4004,
    CancelBookings = 4005,
    CreateRecurringBookings = 4006,
    ManageBookingLocks = 4007,
    ViewBookingCalendar = 4008,

    // Payment Management
    ViewPayments = 5001,
    ProcessPayments = 5002,
    RefundPayments = 5003,
    ManagePaymentMethods = 5004,

    // Media Management
    UploadMedia = 6001,
    ViewMedia = 6002,
    DeleteMedia = 6003,
    ManageMediaStorage = 6004,

    // Notification Management
    SendNotifications = 7001,
    ViewNotifications = 7002,
    ManageNotificationTemplates = 7003,

    // System Administration
    ViewSystemLogs = 8001,
    ManageSystemSettings = 8002,
    BackupRestore = 8003
} 