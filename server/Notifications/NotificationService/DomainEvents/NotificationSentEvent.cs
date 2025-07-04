using Common.DomainEvents;
using Common.Enums;

namespace NotificationService.DomainEvents;

public record NotificationSentEvent(
    string NotificationId,
    string UserId,
    NotificationType Type
) : BaseDomainEvent; 