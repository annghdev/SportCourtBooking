using Common.Base;
using Common.Enums;
using UserService.DomainEvents;

namespace UserService.Entities;

public class User : AggregateRoot
{
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Gender { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public UserRole Role { get; set; } = UserRole.User;
    public string? Address { get; set; }
    public string? Avatar { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsEmailVerified { get; set; }
    public bool IsPhoneVerified { get; set; }
    public DateTimeOffset? LastLoginDate { get; set; }

    // Navigation properties for permission system
    public virtual ICollection<UserPermission> UserPermissions { get; set; } = [];

    public static User Create(string fullName, string email, string? phoneNumber = null, UserRole role = UserRole.User)
    {
        var user = new User
        {
            FullName = fullName,
            Email = email,
            PhoneNumber = phoneNumber,
            Role = role
        };

        user.AddDomainEvent(new UserRegisteredEvent(user.Id, user.Email, user.FullName, user.Role));
        return user;
    }

    public void UpdateProfile(string fullName, string? phoneNumber, string? gender, DateTime? dateOfBirth, string? address)
    {
        FullName = fullName;
        PhoneNumber = phoneNumber;
        Gender = gender;
        DateOfBirth = dateOfBirth;
        Address = address;
        UpdatedDate = DateTimeOffset.UtcNow;

        AddDomainEvent(new UserProfileUpdatedEvent(Id, FullName, PhoneNumber));
    }

    public void ChangeRole(UserRole newRole, string changedBy)
    {
        var oldRole = Role;
        Role = newRole;
        UpdatedBy = changedBy;
        UpdatedDate = DateTimeOffset.UtcNow;

        AddDomainEvent(new UserRoleChangedEvent(Id, oldRole, newRole, changedBy));
    }

    public void GrantPermission(PermissionType permissionType, string? grantedBy = null, DateTime? expiresAt = null, string? notes = null)
    {
        // Check if permission already exists
        var existingPermission = UserPermissions.FirstOrDefault(p => p.PermissionType == permissionType && p.IsActive);
        if (existingPermission != null)
        {
            // Extend expiry if needed
            if (expiresAt.HasValue)
                existingPermission.ExtendExpiry(expiresAt.Value);
            return;
        }

        var userPermission = UserPermission.Grant(Id, permissionType, grantedBy, expiresAt, notes);
        UserPermissions.Add(userPermission);
        UpdatedDate = DateTimeOffset.UtcNow;

        AddDomainEvent(new UserPermissionGrantedEvent(Id, permissionType, grantedBy));
    }

    public void RevokePermission(PermissionType permissionType, string? revokedBy = null)
    {
        var permission = UserPermissions.FirstOrDefault(p => p.PermissionType == permissionType && p.IsActive);
        if (permission != null)
        {
            permission.Revoke();
            UpdatedDate = DateTimeOffset.UtcNow;

            AddDomainEvent(new UserPermissionRevokedEvent(Id, permissionType, revokedBy));
        }
    }

    public bool HasPermission(PermissionType permissionType)
    {
        // Check direct user permissions first
        var directPermission = UserPermissions.FirstOrDefault(p => p.PermissionType == permissionType);
        if (directPermission != null && directPermission.IsValid())
            return true;

        // Check role-based permissions (this would be handled by application service)
        return false;
    }

    public List<PermissionType> GetActivePermissions()
    {
        return UserPermissions
            .Where(p => p.IsValid())
            .Select(p => p.PermissionType)
            .ToList();
    }

    public void SetLastLogin()
    {
        LastLoginDate = DateTimeOffset.UtcNow;
    }

    public void VerifyEmail()
    {
        IsEmailVerified = true;
        UpdatedDate = DateTimeOffset.UtcNow;
    }

    public void VerifyPhone()
    {
        IsPhoneVerified = true;
        UpdatedDate = DateTimeOffset.UtcNow;
    }

    public void Activate(string activatedBy)
    {
        IsActive = true;
        UpdatedBy = activatedBy;
        UpdatedDate = DateTimeOffset.UtcNow;
    }

    public void Deactivate(string deactivatedBy)
    {
        IsActive = false;
        UpdatedBy = deactivatedBy;
        UpdatedDate = DateTimeOffset.UtcNow;
    }
}
