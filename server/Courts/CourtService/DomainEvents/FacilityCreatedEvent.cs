using Common.DomainEvents;

namespace CourtService.DomainEvents;

public record FacilityCreatedEvent(
    string FacilityId,
    string Name,
    string Address
) : BaseDomainEvent; 