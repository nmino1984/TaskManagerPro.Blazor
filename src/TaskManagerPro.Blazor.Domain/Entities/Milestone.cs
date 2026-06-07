using TaskManagerPro.Blazor.Domain.Common;
using TaskManagerPro.Blazor.Domain.Enums;

namespace TaskManagerPro.Blazor.Domain.Entities;

public class Milestone : BaseEntity
{
    private MilestoneStatus _status = MilestoneStatus.Pending;
    private DateTime _targetDate;

    // Required by EF Core — not for direct use
    private Milestone() { }

    public Milestone(string title, string description, DateTime targetDate, Guid taskItemId)
    {
        Title = title;
        Description = description;
        _targetDate = targetDate;
        TaskItemId = taskItemId;
        _status = MilestoneStatus.Pending;
    }

    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;

    // Backing field used by EF Core; immutable after construction to enforce planning discipline
    public DateTime TargetDate => _targetDate;

    // EF Core writes via _status; background jobs may transition this to Overdue
    public MilestoneStatus Status => _status;

    public Guid TaskItemId { get; private set; }
    public TaskItem? TaskItem { get; set; }

    // Status is mutable here so the Application layer can drive Overdue transitions
    // without exposing the backing field directly
    public void Update(string title, string description, DateTime targetDate, MilestoneStatus status)
    {
        Title = title;
        Description = description;
        _targetDate = targetDate;
        _status = status;
    }
}
