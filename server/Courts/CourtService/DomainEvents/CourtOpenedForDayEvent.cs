using Common.DomainEvents;

namespace CourtService.DomainEvents;

public record CourtOpenedForDayEvent(
    string CourtId,
    DayOfWeek DayOfWeek
) : BaseDomainEvent; 