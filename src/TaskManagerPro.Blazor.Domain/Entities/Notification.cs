using TaskManagerPro.Blazor.Domain.Common;
using TaskManagerPro.Blazor.Domain.Enums;

namespace TaskManagerPro.Blazor.Domain.Entities;

/// <summary>
/// Represents an in-app alert delivered to a user in response to a domain event,
/// such as a task approaching its deadline or a subtask being completed.
/// </summary>
public class Notification : BaseEntity
{
    private NotificationStatus _status = NotificationStatus.Unread;

    /// <summary>Required by EF Core for materialisation. Not intended for direct use.</summary>
    private Notification() { }

    /// <summary>
    /// Creates a notification addressed to a user. TaskItemId is optional because
    /// some notifications may be system-level rather than task-specific.
    /// Status defaults to Unread so the user is alerted on next login.
    /// </summary>
    public Notification(string title, string message, Guid userId, Guid? taskItemId = null)
    {
        Title = title;
        Message = message;
        UserId = userId;
        TaskItemId = taskItemId;
        _status = NotificationStatus.Unread;
    }

    /// <summary>Brief heading shown in the notification list.</summary>
    public string Title { get; private set; } = string.Empty;

    /// <summary>Full body of the notification explaining what happened and why it matters.</summary>
    public string Message { get; private set; } = string.Empty;

    /// <summary>
    /// Read/unread state. EF Core writes via _status backing field;
    /// the Application layer is the only intended writer via MarkAsRead commands.
    /// </summary>
    public NotificationStatus Status => _status;

    /// <summary>Foreign key to the recipient user.</summary>
    public Guid UserId { get; private set; }

    /// <summary>Foreign key to the related task, if applicable.</summary>
    public Guid? TaskItemId { get; private set; }

    /// <summary>Navigation property to the recipient user.</summary>
    public AppUser? User { get; set; }

    /// <summary>Navigation property to the related task.</summary>
    public TaskItem? TaskItem { get; set; }

    /// <summary>
    /// Transitions the notification to Read state. Idempotent: calling it on an
    /// already-read notification has no effect.
    /// </summary>
    public void MarkAsRead()
    {
        _status = NotificationStatus.Read;
    }
}
