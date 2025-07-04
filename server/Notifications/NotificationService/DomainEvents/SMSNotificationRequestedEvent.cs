using Common.DomainEvents;

namespace NotificationService.DomainEvents;

public record SMSNotificationRequestedEvent(
    string UserId,
    string PhoneNumber,
    string Message,
    string? TemplateId = null
) : BaseDomainEvent; 