using Common.DomainEvents;

namespace NotificationService.DomainEvents;

public record EmailNotificationRequestedEvent(
    string UserId,
    string Email,
    string Subject,
    string Body,
    string? TemplateId = null
) : BaseDomainEvent; 