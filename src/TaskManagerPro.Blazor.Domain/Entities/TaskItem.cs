using TaskManagerPro.Blazor.Domain.Common;
using TaskManagerPro.Blazor.Domain.Enums;

namespace TaskManagerPro.Blazor.Domain.Entities;

public class TaskItem : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime? DueDate { get; set; }
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public WorkTaskStatus Status { get; set; } = WorkTaskStatus.Pending;

    // Foreign keys
    public Guid UserId { get; set; }

    // Navigation properties
    public AppUser? User { get; set; }
    public ICollection<SubTask> SubTasks { get; set; } = new List<SubTask>();
    public ICollection<Milestone> Milestones { get; set; } = new List<Milestone>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
