using TaskManagerPro.Blazor.Domain.Entities;

namespace TaskManagerPro.Blazor.Domain.Interfaces;

/// <summary>
/// Coordinates multiple repository operations within a single transaction boundary.
/// Application handlers use this interface instead of individual repositories so that
/// all changes in a use-case are committed atomically or rolled back together.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>Repository for task aggregate roots.</summary>
    IRepository<TaskItem> Tasks { get; }

    /// <summary>Repository for subtasks belonging to a task.</summary>
    IRepository<SubTask> SubTasks { get; }

    /// <summary>Repository for milestones belonging to a task.</summary>
    IRepository<Milestone> Milestones { get; }

    /// <summary>Repository for application users.</summary>
    IRepository<AppUser> Users { get; }

    /// <summary>Repository for user notifications.</summary>
    IRepository<Notification> Notifications { get; }

    /// <summary>
    /// Flushes all pending changes to the database in a single transaction.
    /// Should be called once at the end of each Application layer command handler.
    /// </summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
