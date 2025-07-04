using Common.DomainEvents;
using Common.Enums;

namespace UserService.DomainEvents;

public record UserPermissionGrantedEvent(
    string UserId,
    PermissionType PermissionType,
    string? GrantedBy
) : BaseDomainEvent; 