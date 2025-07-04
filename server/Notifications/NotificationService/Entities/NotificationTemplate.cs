using Common.Base;
using Common.Enums;

namespace NotificationService.Entities;

public class NotificationTemplate : AggregateRoot
{
    public required string Name { get; set; }
    public required string Subject { get; set; }
    public required string Body { get; set; }
    public NotificationType Type { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Variables { get; set; } // JSON array of variable names
    public string? SampleData { get; set; } // JSON sample for testing

    public static NotificationTemplate Create(string name, string subject, string body, NotificationType type, string? description = null)
    {
        return new NotificationTemplate
        {
            Name = name,
            Subject = subject,
            Body = body,
            Type = type,
            Description = description
        };
    }

    public void UpdateTemplate(string subject, string body, string? description, string updatedBy)
    {
        Subject = subject;
        Body = body;
        Description = description;
        UpdatedBy = updatedBy;
        UpdatedDate = DateTimeOffset.UtcNow;
    }

    public void SetVariables(IEnumerable<string> variables)
    {
        Variables = System.Text.Json.JsonSerializer.Serialize(variables);
        UpdatedDate = DateTimeOffset.UtcNow;
    }

    public void Activate(string updatedBy)
    {
        IsActive = true;
        UpdatedBy = updatedBy;
        UpdatedDate = DateTimeOffset.UtcNow;
    }

    public void Deactivate(string updatedBy)
    {
        IsActive = false;
        UpdatedBy = updatedBy;
        UpdatedDate = DateTimeOffset.UtcNow;
    }

    public string RenderMessage(Dictionary<string, object> data)
    {
        var message = Body;
        foreach (var item in data)
        {
            message = message.Replace($"{{{item.Key}}}", item.Value?.ToString() ?? "");
        }
        return message;
    }

    public string RenderSubject(Dictionary<string, object> data)
    {
        var subject = Subject;
        foreach (var item in data)
        {
            subject = subject.Replace($"{{{item.Key}}}", item.Value?.ToString() ?? "");
        }
        return subject;
    }
} 