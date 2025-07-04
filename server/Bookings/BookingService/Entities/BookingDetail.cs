using Common.Base;

namespace BookingService.Entities;

public class BookingDetail : BaseEntity
{
    public required string BookingId { get; set; }
    public required string CourtId { get; set; }
    public required string TimeSlotId { get; set; }
    public DateTime PlayDate { get; set; }
    public decimal Price { get; set; }
    public string? CourtName { get; set; } // Denormalized for display
    public string? TimeSlotDisplay { get; set; } // Denormalized for display

    // Navigation property
    public virtual Booking Booking { get; set; } = null!;

    public static BookingDetail Create(string bookingId, string courtId, string timeSlotId, DateTime playDate, decimal price)
    {
        return new BookingDetail
        {
            BookingId = bookingId,
            CourtId = courtId,
            TimeSlotId = timeSlotId,
            PlayDate = playDate,
            Price = price
        };
    }

    public void UpdateDisplayInfo(string courtName, string timeSlotDisplay)
    {
        CourtName = courtName;
        TimeSlotDisplay = timeSlotDisplay;
    }

    public DateTime GetPlayDateTime(TimeOnly startTime)
    {
        return PlayDate.Date.Add(startTime.ToTimeSpan());
    }

    public bool IsOnSameDay(BookingDetail other)
    {
        return PlayDate.Date == other.PlayDate.Date;
    }
} 