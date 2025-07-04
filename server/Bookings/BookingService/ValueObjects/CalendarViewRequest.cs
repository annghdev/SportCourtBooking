namespace BookingService.ValueObjects;

public record CalendarViewRequest
{
    public required string FacilityId { get; init; }
    public string? CourtId { get; init; } // null = tất cả sân
    public DateTime FromDate { get; init; }
    public DateTime ToDate { get; init; }
    public string? UserId { get; init; } // để filter booking của user
    public bool ShowUnavailable { get; init; } = true;
    public bool ShowPricing { get; init; } = true;

    public static CalendarViewRequest ForFacility(string facilityId, DateTime fromDate, DateTime toDate)
    {
        return new CalendarViewRequest
        {
            FacilityId = facilityId,
            FromDate = fromDate,
            ToDate = toDate
        };
    }

    public static CalendarViewRequest ForCourt(string facilityId, string courtId, DateTime fromDate, DateTime toDate)
    {
        return new CalendarViewRequest
        {
            FacilityId = facilityId,
            CourtId = courtId,
            FromDate = fromDate,
            ToDate = toDate
        };
    }

    public static CalendarViewRequest ForUser(string facilityId, string userId, DateTime fromDate, DateTime toDate)
    {
        return new CalendarViewRequest
        {
            FacilityId = facilityId,
            UserId = userId,
            FromDate = fromDate,
            ToDate = toDate,
            ShowUnavailable = false
        };
    }

    public TimeSpan GetDateRange()
    {
        return ToDate - FromDate;
    }

    public bool IsValidDateRange()
    {
        return FromDate <= ToDate && GetDateRange().Days <= 365; // Max 1 year
    }

    public List<DateTime> GetDateList()
    {
        var dates = new List<DateTime>();
        var currentDate = FromDate.Date;
        
        while (currentDate <= ToDate.Date)
        {
            dates.Add(currentDate);
            currentDate = currentDate.AddDays(1);
        }
        
        return dates;
    }
} 