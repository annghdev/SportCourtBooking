using Common.DomainEvents;

namespace CourtService.DomainEvents;

public record CourtUnavailableEvent(
    string CourtId,
    string? Reason
) : BaseDomainEvent; 