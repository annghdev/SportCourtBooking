using Common.Base;

namespace CourtService.Entities;

public class TimeSlot : BaseEntity
{
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public int DurationInMinutes { get; set; }
    public bool IsActive { get; set; } = true;

    public static TimeSlot Create(TimeOnly startTime, TimeOnly endTime)
    {
        var duration = (int)(endTime - startTime).TotalMinutes;
        
        return new TimeSlot
        {
            StartTime = startTime,
            EndTime = endTime,
            DurationInMinutes = duration
        };
    }

    public string GetDisplayName()
    {
        return $"{StartTime:HH:mm} - {EndTime:HH:mm}";
    }

    public bool IsOverlapping(TimeSlot other)
    {
        return StartTime < other.EndTime && EndTime > other.StartTime;
    }
}
