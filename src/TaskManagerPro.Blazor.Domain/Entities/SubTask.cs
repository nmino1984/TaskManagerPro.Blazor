using TaskManagerPro.Blazor.Domain.Common;

namespace TaskManagerPro.Blazor.Domain.Entities;

/// <summary>
/// An atomic step within a parent TaskItem. Subtasks allow partial completion
/// tracking without elevating every minor step to a full task.
/// </summary>
public class SubTask : BaseEntity
{
    private bool _isCompleted;

    // Required by EF Core — not for direct use
    private SubTask() { }

    public SubTask(string title, string description, Guid taskItemId)
    {
        Title = title;
        Description = description;
        TaskItemId = taskItemId;
        _isCompleted = false;
    }

    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;

    // EF Core writes via _isCompleted; toggled by Application layer commands only
    public bool IsCompleted => _isCompleted;

    public Guid TaskItemId { get; private set; }
    public TaskItem? TaskItem { get; set; }

    public void Update(string title, string description, bool isCompleted)
    {
        Title = title;
        Description = description;
        _isCompleted = isCompleted;
    }
}
