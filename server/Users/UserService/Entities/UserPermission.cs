using Common.Base;
using Common.Enums;

namespace UserService.Entities;

public class UserPermission : BaseEntity
{
    public required string UserId { get; set; }
    public required PermissionType PermissionType { get; set; }
    public string? GrantedBy { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Notes { get; set; }

    // Navigation properties
    public virtual User User { get; set; } = null!;

    public static UserPermission Grant(string userId, PermissionType permissionType, string? grantedBy = null, DateTime? expiresAt = null, string? notes = null)
    {
        return new UserPermission
        {
            UserId = userId,
            PermissionType = permissionType,
            GrantedBy = grantedBy,
            ExpiresAt = expiresAt,
            Notes = notes
        };
    }

    public void Revoke()
    {
        IsActive = false;
    }

    public void ExtendExpiry(DateTime newExpiryDate)
    {
        ExpiresAt = newExpiryDate;
    }

    public bool IsExpired()
    {
        return ExpiresAt.HasValue && DateTime.UtcNow > ExpiresAt.Value;
    }

    public bool IsValid()
    {
        return IsActive && !IsExpired();
    }
} 