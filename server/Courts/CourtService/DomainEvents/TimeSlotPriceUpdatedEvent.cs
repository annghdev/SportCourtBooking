using Common.DomainEvents;

namespace CourtService.DomainEvents;

public record TimeSlotPriceUpdatedEvent(
    string CourtId,
    string TimeSlotId,
    decimal NewPrice,
    DayOfWeek? DayOfWeek
) : BaseDomainEvent; 