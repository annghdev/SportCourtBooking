using Common.DomainEvents;

namespace UserService.DomainEvents;

public record UserProfileUpdatedEvent(
    string UserId,
    string FullName,
    string? PhoneNumber
) : BaseDomainEvent; 