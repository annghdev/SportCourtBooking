using Common.DomainEvents;
using Common.Enums;

namespace UserService.DomainEvents;

public record UserPermissionRevokedEvent(
    string UserId,
    PermissionType PermissionType,
    string? RevokedBy
) : BaseDomainEvent; 