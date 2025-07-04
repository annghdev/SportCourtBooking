using Common.DomainEvents;

namespace CourtService.DomainEvents;

public record CourtMaintenanceCancelledEvent(
    string CourtId,
    string MaintenanceId,
    string Title,
    string? Reason
) : BaseDomainEvent; 