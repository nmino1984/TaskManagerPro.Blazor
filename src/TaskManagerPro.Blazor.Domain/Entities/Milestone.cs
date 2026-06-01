using TaskManagerPro.Blazor.Domain.Common;
using TaskManagerPro.Blazor.Domain.Enums;

namespace TaskManagerPro.Blazor.Domain.Entities;

public class Milestone : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime TargetDate { get; set; }
    public MilestoneStatus Status { get; set; } = MilestoneStatus.Pending;

    // Foreign keys
    public Guid TaskItemId { get; set; }

    // Navigation properties
    public TaskItem? TaskItem { get; set; }
}
