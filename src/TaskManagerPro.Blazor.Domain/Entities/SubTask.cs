using TaskManagerPro.Blazor.Domain.Common;

namespace TaskManagerPro.Blazor.Domain.Entities;

/// <summary>
/// Represents an atomic step within a parent TaskItem.
/// Breaking work into subtasks allows partial completion tracking
/// without promoting minor steps to full tasks.
/// </summary>
public class SubTask : BaseEntity
{
    private bool _isCompleted;

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
        _isCompleted = false;
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
    /// Indicates whether this step has been finished. EF Core writes via _isCompleted
    /// backing field; toggled by Application layer commands only.
    /// </summary>
    public bool IsCompleted => _isCompleted;

    /// <summary>
    /// Foreign key to the parent task.
    /// </summary>
    public Guid TaskItemId { get; private set; }

    /// <summary>Navigation property to the parent task.</summary>
    public TaskItem? TaskItem { get; set; }
}
