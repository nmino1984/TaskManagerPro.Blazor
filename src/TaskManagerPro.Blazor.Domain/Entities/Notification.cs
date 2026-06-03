using TaskManagerPro.Blazor.Domain.Common;
using TaskManagerPro.Blazor.Domain.Enums;

namespace TaskManagerPro.Blazor.Domain.Entities;

/// <summary>
/// An in-app alert delivered to a user in response to a domain event,
/// such as a task approaching its deadline.
/// </summary>
public class Notification : BaseEntity
{
    private NotificationStatus _status = NotificationStatus.Unread;

    // Required by EF Core — not for direct use
    private Notification() { }

    /// <summary>
    /// TaskItemId is optional because some notifications may be system-level
    /// rather than tied to a specific task.
    /// </summary>
    public Notification(string title, string message, Guid userId, Guid? taskItemId = null)
    {
        Title = title;
        Message = message;
        UserId = userId;
        TaskItemId = taskItemId;
        _status = NotificationStatus.Unread;
    }

    public string Title { get; private set; } = string.Empty;
    public string Message { get; private set; } = string.Empty;

    // EF Core writes via _status; only the Application layer should call MarkAsRead
    public NotificationStatus Status => _status;

    public Guid UserId { get; private set; }
    public Guid? TaskItemId { get; private set; }
    public AppUser? User { get; set; }
    public TaskItem? TaskItem { get; set; }

    // Idempotent — safe to call even if already read
    public void MarkAsRead()
    {
        _status = NotificationStatus.Read;
    }
}
