using Common.DomainEvents;

namespace CourtService.DomainEvents;

public record CourtClosedForDayEvent(
    string CourtId,
    DayOfWeek DayOfWeek,
    string? Reason
) : BaseDomainEvent; 