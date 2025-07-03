namespace Common.Base;

public abstract class AggregateRoot : BaseEntity
{
    public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedDate { get; set; }
    public DateTimeOffset? DeletedDate { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public string? DeletedBy { get; set; }

    public bool IsDeleted => DeletedDate != null;
}
