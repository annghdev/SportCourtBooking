using Common.DomainEvents;

namespace CourtService.DomainEvents;

public record CourtMaintenanceCompletedEvent(
    string CourtId,
    string MaintenanceId,
    string Title,
    decimal? ActualCost
) : BaseDomainEvent; 