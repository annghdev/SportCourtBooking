using Common.Base;
using Common.Enums;
using BookingService.DomainEvents;

namespace BookingService.Entities;

public class Booking : AggregateRoot
{
    public required string UserId { get; set; }
    public required string FacilityId { get; set; }
    public BookingType Type { get; set; }
    public BookingStatus Status { get; set; } = BookingStatus.Pending;
    public DateTime BookingDate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal FinalAmount { get; set; }
    public string? Notes { get; set; }
    public string? CancellationReason { get; set; }
    public DateTime? PaymentDeadline { get; set; }

    // Navigation properties
    public virtual ICollection<BookingDetail> BookingDetails { get; set; } = [];

    public static Booking Create(string userId, string facilityId, BookingType type, DateTime bookingDate, string? notes = null)
    {
        var booking = new Booking
        {
            UserId = userId,
            FacilityId = facilityId,
            Type = type,
            BookingDate = bookingDate,
            Notes = notes
        };

        // Set payment deadline for online payment methods (15 minutes)
        booking.PaymentDeadline = DateTime.UtcNow.AddMinutes(15);

        booking.AddDomainEvent(new BookingCreatedEvent(booking.Id, booking.UserId, booking.FacilityId, booking.Type));
        return booking;
    }

    public void AddBookingDetail(string courtId, string timeSlotId, DateTime playDate, decimal price)
    {
        var detail = BookingDetail.Create(Id, courtId, timeSlotId, playDate, price);
        BookingDetails.Add(detail);
        RecalculateTotal();
    }

    public void RecalculateTotal()
    {
        TotalAmount = BookingDetails.Sum(d => d.Price);
        FinalAmount = TotalAmount - DiscountAmount;
        UpdatedDate = DateTimeOffset.UtcNow;
    }

    public void ApplyDiscount(decimal discountAmount, string updatedBy)
    {
        DiscountAmount = discountAmount;
        FinalAmount = TotalAmount - DiscountAmount;
        UpdatedBy = updatedBy;
        UpdatedDate = DateTimeOffset.UtcNow;
    }

    public void Confirm(string confirmedBy)
    {
        Status = BookingStatus.Confirmed;
        UpdatedBy = confirmedBy;
        UpdatedDate = DateTimeOffset.UtcNow;

        AddDomainEvent(new BookingConfirmedEvent(Id, UserId, FacilityId, FinalAmount));
    }

    public void Cancel(string reason, string cancelledBy)
    {
        Status = BookingStatus.Cancelled;
        CancellationReason = reason;
        UpdatedBy = cancelledBy;
        UpdatedDate = DateTimeOffset.UtcNow;

        AddDomainEvent(new BookingCancelledEvent(Id, UserId, reason));
    }

    public void Complete(string completedBy)
    {
        Status = BookingStatus.Completed;
        UpdatedBy = completedBy;
        UpdatedDate = DateTimeOffset.UtcNow;

        AddDomainEvent(new BookingCompletedEvent(Id, UserId, FacilityId));
    }

    public void MarkAsInProgress()
    {
        Status = BookingStatus.InProgress;
        UpdatedDate = DateTimeOffset.UtcNow;
    }

    public void MarkAsNoShow(string updatedBy)
    {
        Status = BookingStatus.NoShow;
        UpdatedBy = updatedBy;
        UpdatedDate = DateTimeOffset.UtcNow;
    }

    public bool IsPaymentOverdue()
    {
        return PaymentDeadline.HasValue && DateTime.UtcNow > PaymentDeadline.Value && Status == BookingStatus.Pending;
    }
}
