using Common.DomainEvents;

namespace CourtService.DomainEvents;

public record CourtMaintenanceRescheduledEvent(
    string CourtId,
    string MaintenanceId,
    DateTime OldStartTime,
    DateTime OldEndTime,
    DateTime NewStartTime,
    DateTime NewEndTime
) : BaseDomainEvent; 