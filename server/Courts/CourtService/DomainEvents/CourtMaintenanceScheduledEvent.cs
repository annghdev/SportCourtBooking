using Common.DomainEvents;
using Common.Enums;

namespace CourtService.DomainEvents;

public record CourtMaintenanceScheduledEvent(
    string CourtId,
    string MaintenanceId,
    DateTime StartTime,
    DateTime EndTime,
    MaintenanceType Type
) : BaseDomainEvent; 