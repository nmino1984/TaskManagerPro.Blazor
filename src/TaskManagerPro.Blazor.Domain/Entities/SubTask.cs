using TaskManagerPro.Blazor.Domain.Common;

namespace TaskManagerPro.Blazor.Domain.Entities;

/// <summary>
/// Represents an atomic step within a parent TaskItem.
/// Breaking work into subtasks allows partial completion tracking
/// without promoting minor steps to full tasks.
/// </summary>
public class SubTask : BaseEntity
{
    /// <summary>
    /// Required by EF Core for materialisation. Not intended for direct use.
    /// </summary>
    private SubTask() { }

    /// <summary>
    /// Creates a subtask bound to an existing task.
    /// Completion defaults to false because the work has not been done yet.
    /// </summary>
    public SubTask(string title, string description, Guid taskItemId)
    {
        Title = title;
        Description = description;
        TaskItemId = taskItemId;
        IsCompleted = false;
    }

    /// <summary>
    /// Short label describing the individual step.
    /// </summary>
    public string Title { get; private set; } = string.Empty;

    /// <summary>
    /// Additional detail about what completing this step requires.
    /// </summary>
    public string Description { get; private set; } = string.Empty;

    /// <summary>
    /// Indicates whether this step has been finished. Internal setter lets the
    /// Application layer toggle completion without exposing it to API callers.
    /// </summary>
    public bool IsCompleted { get; internal set; } = false;

    /// <summary>
    /// Foreign key to the parent task.
    /// </summary>
    public Guid TaskItemId { get; private set; }

    /// <summary>Navigation property to the parent task.</summary>
    public TaskItem? TaskItem { get; set; }
}
