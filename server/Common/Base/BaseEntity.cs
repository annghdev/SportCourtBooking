using Common.Utils;
using Cortex.Mediator.Notifications;

namespace Common.Base;

public abstract class BaseEntity
{
    public string Id { get; set; } = UUID.CreateAlphanumericSeries();
    private readonly List<INotification> _domainEvents = [];
    public IReadOnlyCollection<INotification> DomainEvents => _domainEvents;

    public void AddDomainEvent(INotification domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
    public void ClearDomainEvent()
    {
        _domainEvents.Clear();
    }
}
