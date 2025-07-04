using Common.Base;
using CourtService.DomainEvents;

namespace CourtService.Entities;

public class CourtOperatingHours : BaseEntity
{
    public required string CourtId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeOnly OpenTime { get; set; }
    public TimeOnly CloseTime { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsClosed { get; set; } = false; // Đóng cửa hoàn toàn trong ngày này
    public string? Notes { get; set; }

    // Navigation property
    public virtual Court Court { get; set; } = null!;

    public static CourtOperatingHours Create(string courtId, DayOfWeek dayOfWeek, TimeOnly openTime, TimeOnly closeTime, string? notes = null)
    {
        var operatingHours = new CourtOperatingHours
        {
            CourtId = courtId,
            DayOfWeek = dayOfWeek,
            OpenTime = openTime,
            CloseTime = closeTime,
            Notes = notes
        };

        operatingHours.AddDomainEvent(new CourtOperatingHoursUpdatedEvent(courtId, dayOfWeek, openTime, closeTime, true));
        return operatingHours;
    }

    public void UpdateHours(TimeOnly openTime, TimeOnly closeTime, string? notes = null)
    {
        OpenTime = openTime;
        CloseTime = closeTime;
        Notes = notes;

        AddDomainEvent(new CourtOperatingHoursUpdatedEvent(CourtId, DayOfWeek, openTime, closeTime, IsActive));
    }

    public void CloseForDay(string? reason = null)
    {
        IsClosed = true;
        Notes = reason;

        AddDomainEvent(new CourtClosedForDayEvent(CourtId, DayOfWeek, reason));
    }

    public void OpenForDay()
    {
        IsClosed = false;

        AddDomainEvent(new CourtOpenedForDayEvent(CourtId, DayOfWeek));
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public bool IsWithinOperatingHours(TimeOnly time)
    {
        if (!IsActive || IsClosed)
            return false;

        return time >= OpenTime && time <= CloseTime;
    }

    public bool IsOperatingOnDate(DateTime date)
    {
        return IsActive && !IsClosed && date.DayOfWeek == DayOfWeek;
    }

    public TimeSpan GetOperatingDuration()
    {
        return CloseTime.ToTimeSpan() - OpenTime.ToTimeSpan();
    }
} 