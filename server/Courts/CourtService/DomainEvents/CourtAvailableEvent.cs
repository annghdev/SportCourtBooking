using Common.DomainEvents;

namespace CourtService.DomainEvents;

public record CourtAvailableEvent(
    string CourtId
) : BaseDomainEvent; 