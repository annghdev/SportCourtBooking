using Common.Base;
using Common.Enums;

namespace BookingService.Entities;

public class RecurringBookingInstance : BaseEntity
{
    public required string RecurringBookingId { get; set; }
    public DateTime PlayDate { get; set; }
    public BookingStatus Status { get; set; } = BookingStatus.Confirmed;
    public decimal Price { get; set; }
    public string? BookingId { get; set; } // Link to actual booking if created
    public string? CancellationReason { get; set; }
    
    // Navigation property
    public virtual RecurringBooking RecurringBooking { get; set; } = null!;

    public static RecurringBookingInstance Create(string recurringBookingId, DateTime playDate, decimal price)
    {
        return new RecurringBookingInstance
        {
            RecurringBookingId = recurringBookingId,
            PlayDate = playDate,
            Price = price
        };
    }

    public void LinkToBooking(string bookingId)
    {
        BookingId = bookingId;
        Status = BookingStatus.Confirmed;
    }

    public void Cancel(string reason)
    {
        Status = BookingStatus.Cancelled;
        CancellationReason = reason;
    }

    public void Complete()
    {
        Status = BookingStatus.Completed;
    }

    public void MarkAsNoShow()
    {
        Status = BookingStatus.NoShow;
    }

    public bool IsUpcoming()
    {
        return PlayDate.Date >= DateTime.Today && Status == BookingStatus.Confirmed;
    }
} 