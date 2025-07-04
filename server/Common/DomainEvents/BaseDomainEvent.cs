using Cortex.Mediator.Notifications;

namespace Common.DomainEvents;

public abstract record BaseDomainEvent : INotification
{
    public string Id { get; init; } = Guid.NewGuid().ToString();
    public DateTimeOffset OccurredOn { get; init; } = DateTimeOffset.UtcNow;
} 