using Common.DomainEvents;
using Common.Enums;

namespace UserService.DomainEvents;

public record UserRoleChangedEvent(
    string UserId,
    UserRole OldRole,
    UserRole NewRole,
    string ChangedBy
) : BaseDomainEvent; 