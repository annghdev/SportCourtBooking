using Common.DomainEvents;
using Common.Enums;

namespace NotificationService.DomainEvents;

public record NotificationCreatedEvent(
    string NotificationId,
    string UserId,
    string Title,
    string Message,
    NotificationType Type
) : BaseDomainEvent; 