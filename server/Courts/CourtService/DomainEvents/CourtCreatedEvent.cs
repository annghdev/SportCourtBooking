using Common.DomainEvents;
using Common.Enums;

namespace CourtService.DomainEvents;

public record CourtCreatedEvent(
    string CourtId,
    string Name,
    string FacilityId,
    CourtType Type
) : BaseDomainEvent; 