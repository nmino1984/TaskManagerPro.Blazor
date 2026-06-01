using TaskManagerPro.Blazor.Domain.Common;
using TaskManagerPro.Blazor.Domain.Enums;

namespace TaskManagerPro.Blazor.Domain.Entities;

public class Notification : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationStatus Status { get; set; } = NotificationStatus.Unread;

    // Foreign keys
    public Guid UserId { get; set; }
    public Guid? TaskItemId { get; set; }

    // Navigation properties
    public AppUser? User { get; set; }
    public TaskItem? TaskItem { get; set; }
}
