using TaskManagerPro.Blazor.Domain.Common;

namespace TaskManagerPro.Blazor.Domain.Entities;

public class SubTask : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsCompleted { get; set; } = false;

    // Foreign keys
    public Guid TaskItemId { get; set; }

    // Navigation properties
    public TaskItem? TaskItem { get; set; }
}
