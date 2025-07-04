using Common.Base;
using CourtService.DomainEvents;

namespace CourtService.Entities;

public class CourtPricing : BaseEntity
{
    public required string CourtId { get; set; }
    public required string TimeSlotId { get; set; }
    public decimal Price { get; set; }
    public DayOfWeek? DayOfWeek { get; set; } // null = áp dụng cho tất cả các ngày
    public DateTime? EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual Court Court { get; set; } = null!;
    public virtual TimeSlot TimeSlot { get; set; } = null!;

    public static CourtPricing Create(string courtId, string timeSlotId, decimal price, 
        DayOfWeek? dayOfWeek = null, DateTime? effectiveFrom = null, DateTime? effectiveTo = null)
    {
        var pricing = new CourtPricing
        {
            CourtId = courtId,
            TimeSlotId = timeSlotId,
            Price = price,
            DayOfWeek = dayOfWeek,
            EffectiveFrom = effectiveFrom,
            EffectiveTo = effectiveTo
        };

        pricing.AddDomainEvent(new TimeSlotPriceUpdatedEvent(courtId, timeSlotId, price, dayOfWeek));
        return pricing;
    }

    public void UpdatePrice(decimal newPrice)
    {
        Price = newPrice;

        AddDomainEvent(new TimeSlotPriceUpdatedEvent(CourtId, TimeSlotId, newPrice, DayOfWeek));
    }

    public bool IsValidForDate(DateTime date)
    {
        // Kiểm tra ngày trong tuần nếu có
        if (DayOfWeek.HasValue && date.DayOfWeek != DayOfWeek.Value)
            return false;

        // Kiểm tra thời gian hiệu lực
        if (EffectiveFrom.HasValue && date < EffectiveFrom.Value)
            return false;

        if (EffectiveTo.HasValue && date > EffectiveTo.Value)
            return false;

        return IsActive;
    }
} 