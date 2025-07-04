using Common.Base;
using Common.Enums;
using NotificationService.DomainEvents;

namespace NotificationService.Entities;

public class Notification : AggregateRoot
{
    public required string UserId { get; set; }
    public NotificationType Type { get; set; }
    public string? RecipientAddress { get; set; } // Email address, phone number, etc.
    public required string Subject { get; set; }
    public required string Body { get; set; }
    public bool IsSent { get; set; } = false;
    public DateTime? SentAt { get; set; }
    public int RetryCount { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime? ScheduledFor { get; set; }

    public static Notification Create(string userId, NotificationType type, string recipient, string subject, string body, DateTime? scheduledFor = null)
    {
        var notification = new Notification
        {
            UserId = userId,
            Type = type,
            RecipientAddress = recipient,
            Subject = subject,
            Body = body,
            ScheduledFor = scheduledFor
        };

        notification.AddDomainEvent(new NotificationCreatedEvent(notification.Id, userId, notification.Subject, notification.Body, type));
        return notification;
    }

    public void MarkAsSent()
    {
        IsSent = true;
        SentAt = DateTime.UtcNow;
        ErrorMessage = null;
        UpdatedDate = DateTimeOffset.UtcNow;
        AddDomainEvent(new NotificationSentEvent(Id, UserId, Type));
    }

    public void MarkAsFailed(string error)
    {
        IsSent = false;
        ErrorMessage = error;
        RetryCount++;
        UpdatedDate = DateTimeOffset.UtcNow;
    }

    public bool CanRetry(int maxRetries)
    {
        return !IsSent && RetryCount < maxRetries;
    }
} 