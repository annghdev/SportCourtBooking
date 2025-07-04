using Common.DomainEvents;

namespace BookingService.DomainEvents;

public record RecurringBookingCreatedEvent(
    string RecurringBookingId,
    string UserId,
    string FacilityId,
    string CourtId,
    string TimeSlotId,
    DateTime StartDate,
    DateTime? EndDate,
    IEnumerable<DayOfWeek> DaysOfWeek
) : BaseDomainEvent; 