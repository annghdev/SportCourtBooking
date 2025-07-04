using Common.DomainEvents;
using Common.Enums;

namespace UserService.DomainEvents;

public record UserRegisteredEvent(
    string UserId,
    string Email,
    string FullName,
    UserRole Role
) : BaseDomainEvent; 