using Common.Base;
using CourtService.DomainEvents;

namespace CourtService.Entities;

public class Facility : AggregateRoot
{
    public required string Name { get; set; }
    public required string Address { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? Description { get; set; }
    public TimeOnly OpenTime { get; set; } = new(6, 0);    // 6:00 AM
    public TimeOnly CloseTime { get; set; } = new(22, 0);  // 10:00 PM
    public bool IsActive { get; set; } = true;
    public string? ManagerId { get; set; }

    // Navigation properties
    // public virtual ICollection<Court> Courts { get; set; } = [];

    public static Facility Create(string name, string address, string? phoneNumber = null, string? email = null)
    {
        var facility = new Facility
        {
            Name = name,
            Address = address,
            PhoneNumber = phoneNumber,
            Email = email
        };

        facility.AddDomainEvent(new FacilityCreatedEvent(facility.Id, facility.Name, facility.Address));
        return facility;
    }

    public void UpdateInfo(string name, string address, string? phoneNumber, string? email, string? description)
    {
        Name = name;
        Address = address;
        PhoneNumber = phoneNumber;
        Email = email;
        Description = description;
        UpdatedDate = DateTimeOffset.UtcNow;
    }

    public void SetOperatingHours(TimeOnly openTime, TimeOnly closeTime)
    {
        OpenTime = openTime;
        CloseTime = closeTime;
        UpdatedDate = DateTimeOffset.UtcNow;
    }

    public void SetManager(string managerId, string updatedBy)
    {
        ManagerId = managerId;
        UpdatedBy = updatedBy;
        UpdatedDate = DateTimeOffset.UtcNow;
    }

    public void Deactivate(string deactivatedBy)
    {
        IsActive = false;
        UpdatedBy = deactivatedBy;
        UpdatedDate = DateTimeOffset.UtcNow;
    }
}
