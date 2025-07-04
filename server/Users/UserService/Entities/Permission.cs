using Common.Base;
using Common.Enums;

namespace UserService.Entities;

public class Permission : BaseEntity
{
    public required PermissionType Type { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public string? Category { get; set; }
    public bool IsActive { get; set; } = true;

    public static Permission Create(PermissionType type, string name, string description, string? category = null)
    {
        return new Permission
        {
            Type = type,
            Name = name,
            Description = description,
            Category = category
        };
    }

    public void UpdateInfo(string name, string description, string? category)
    {
        Name = name;
        Description = description;
        Category = category;
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