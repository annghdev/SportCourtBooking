using Common.DomainEvents;
using Common.Enums;

namespace BookingService.DomainEvents;

public record CalendarSlotStatusChangedEvent(
    string SlotId,
    string CourtId,
    string TimeSlotId,
    DateTime SlotDate,
    SlotStatus OldStatus,
    SlotStatus NewStatus,
    string? UserId,
    string? BookingId,
    string? Reason
) : BaseDomainEvent; 