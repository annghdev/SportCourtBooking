using Common.Base;
using Common.Enums;

namespace UserService.Entities;

public class RolePermission : BaseEntity
{
    public required UserRole Role { get; set; }
    public required PermissionType PermissionType { get; set; }
    public bool IsActive { get; set; } = true;
    public string? CreatedBy { get; set; }

    public static RolePermission Create(UserRole role, PermissionType permissionType, string? createdBy = null)
    {
        return new RolePermission
        {
            Role = role,
            PermissionType = permissionType,
            CreatedBy = createdBy
        };
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
} 