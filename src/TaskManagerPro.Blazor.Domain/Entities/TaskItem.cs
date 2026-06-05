using TaskManagerPro.Blazor.Domain.Common;
using TaskManagerPro.Blazor.Domain.Enums;

namespace TaskManagerPro.Blazor.Domain.Entities;

/// <summary>
/// Core aggregate root. Represents a unit of work assigned to a user,
/// with lifecycle managed through explicit status transitions.
/// </summary>
public class TaskItem : BaseEntity
{
    private WorkTaskStatus _status = WorkTaskStatus.Pending;
    private DateTime? _dueDate;

    // Required by EF Core — not for direct use
    private TaskItem() { }

    public TaskItem(string title, string description, DateTime? dueDate, TaskPriority priority, Guid userId, Guid? assignedToUserId = null)
    {
        Title = title;
        Description = description;
        _dueDate = dueDate;
        Priority = priority;
        UserId = userId;
        AssignedToUserId = assignedToUserId;
        _status = WorkTaskStatus.Pending;
    }

    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;

    // Exposed via backing field so EF Core can hydrate it while keeping the setter off the public API
    public DateTime? DueDate => _dueDate;

    public TaskPriority Priority { get; private set; } = TaskPriority.Medium;

    // EF Core writes via _status; transitions go through the methods below, never via direct assignment
    public WorkTaskStatus Status => _status;

    public Guid UserId { get; private set; }
    public Guid? AssignedToUserId { get; private set; }
    public AppUser? User { get; set; }
    public IReadOnlyCollection<SubTask> SubTasks { get; set; } = new List<SubTask>();
    public IReadOnlyCollection<Milestone> Milestones { get; set; } = new List<Milestone>();
    public IReadOnlyCollection<Notification> Notifications { get; set; } = new List<Notification>();

    // Centralised so mutation is never scattered across multiple Application handlers
    public void Update(string title, string description, DateTime? dueDate, TaskPriority priority, Guid? assignedToUserId = null)
    {
        Title = title;
        Description = description;
        _dueDate = dueDate;
        Priority = priority;
        AssignedToUserId = assignedToUserId;
    }

    // Completed and Cancelled are terminal — caller must ResetToPending() before resuming
    public void StartProgress()
    {
        if (_status == WorkTaskStatus.Completed || _status == WorkTaskStatus.Cancelled)
            throw new InvalidOperationException($"Cannot start a task that is {_status}.");
        _status = WorkTaskStatus.InProgress;
    }

    // Cancelled tasks must be reset first; completing from Pending is intentionally allowed
    public void Complete()
    {
        if (_status == WorkTaskStatus.Cancelled)
            throw new InvalidOperationException("Cannot complete a cancelled task.");
        _status = WorkTaskStatus.Completed;
    }

    public void Cancel()
    {
        if (_status == WorkTaskStatus.Completed)
            throw new InvalidOperationException("Cannot cancel a completed task.");
        _status = WorkTaskStatus.Cancelled;
    }

    public void ResetToPending()
    {
        _status = WorkTaskStatus.Pending;
    }
}
