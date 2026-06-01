using TaskManagerPro.Blazor.Domain.Common;
using TaskManagerPro.Blazor.Domain.Enums;

namespace TaskManagerPro.Blazor.Domain.Entities;

/// <summary>
/// Represents a significant checkpoint within a task's timeline.
/// Milestones differ from subtasks in that they carry a target date and
/// can become Overdue, enabling deadline-aware progress reporting.
/// </summary>
public class Milestone : BaseEntity
{
    private MilestoneStatus _status = MilestoneStatus.Pending;
    private DateTime _targetDate;

    /// <summary>
    /// Required by EF Core for materialisation. Not intended for direct use.
    /// </summary>
    private Milestone() { }

    /// <summary>
    /// Creates a milestone associated with a task.
    /// Status defaults to Pending because no work has started at creation time.
    /// </summary>
    public Milestone(string title, string description, DateTime targetDate, Guid taskItemId)
    {
        Title = title;
        Description = description;
        _targetDate = targetDate;
        TaskItemId = taskItemId;
        _status = MilestoneStatus.Pending;
    }

    /// <summary>
    /// Short label identifying this checkpoint.
    /// </summary>
    public string Title { get; private set; } = string.Empty;

    /// <summary>
    /// Detailed description of what this milestone represents.
    /// </summary>
    public string Description { get; private set; } = string.Empty;

    /// <summary>
    /// The date by which this milestone should be reached. EF Core writes via
    /// _targetDate backing field; immutable after construction to enforce planning discipline.
    /// </summary>
    public DateTime TargetDate => _targetDate;

    /// <summary>
    /// Lifecycle state of the milestone. EF Core writes via _status backing field;
    /// background jobs may transition to Overdue when TargetDate is exceeded.
    /// </summary>
    public MilestoneStatus Status => _status;

    /// <summary>
    /// Foreign key to the parent task.
    /// </summary>
    public Guid TaskItemId { get; private set; }

    /// <summary>Navigation property to the parent task.</summary>
    public TaskItem? TaskItem { get; set; }
}
