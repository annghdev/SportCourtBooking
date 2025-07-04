using Common.DomainEvents;

namespace CourtService.DomainEvents;

public record CourtOperatingHoursUpdatedEvent(
    string CourtId,
    DayOfWeek DayOfWeek,
    TimeOnly OpenTime,
    TimeOnly CloseTime,
    bool IsActive
) : BaseDomainEvent; 