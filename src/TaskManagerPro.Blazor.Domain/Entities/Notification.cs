using TaskManagerPro.Blazor.Domain.Common;
using TaskManagerPro.Blazor.Domain.Enums;

namespace TaskManagerPro.Blazor.Domain.Entities;

/// <summary>
/// Represents an in-app alert delivered to a user in response to a domain event,
/// such as a task approaching its deadline or a subtask being completed.
/// </summary>
public class Notification : BaseEntity
{
    /// <summary>
    /// Required by EF Core for materialisation. Not intended for direct use.
    /// </summary>
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
        Status = NotificationStatus.Unread;
    }

    /// <summary>
    /// Brief heading shown in the notification list.
    /// </summary>
    public string Title { get; private set; } = string.Empty;

    /// <summary>
    /// Full body of the notification explaining what happened and why it matters.
    /// </summary>
    public string Message { get; private set; } = string.Empty;

    /// <summary>
    /// Read/unread state. Internal setter lets the Application layer mark as Read
    /// without allowing arbitrary external mutation.
    /// </summary>
    public NotificationStatus Status { get; internal set; } = NotificationStatus.Unread;

    /// <summary>
    /// Foreign key to the recipient user.
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Foreign key to the related task, if applicable.
    /// </summary>
    public Guid? TaskItemId { get; private set; }

    /// <summary>Navigation property to the recipient user.</summary>
    public AppUser? User { get; set; }

    /// <summary>Navigation property to the related task.</summary>
    public TaskItem? TaskItem { get; set; }
}
