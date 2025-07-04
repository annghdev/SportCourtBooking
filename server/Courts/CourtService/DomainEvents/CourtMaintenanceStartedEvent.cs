using Common.DomainEvents;

namespace CourtService.DomainEvents;

public record CourtMaintenanceStartedEvent(
    string CourtId,
    string MaintenanceId,
    string Title
) : BaseDomainEvent; 