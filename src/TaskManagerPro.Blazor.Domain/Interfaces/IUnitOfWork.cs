using TaskManagerPro.Blazor.Domain.Entities;

namespace TaskManagerPro.Blazor.Domain.Interfaces;

/// <summary>
/// Coordinates multiple repository operations within a single transaction.
/// Application handlers use this instead of individual repositories so all
/// changes in a use-case commit atomically or roll back together.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    IRepository<TaskItem> Tasks { get; }
    IRepository<SubTask> SubTasks { get; }
    IRepository<Milestone> Milestones { get; }
    IRepository<AppUser> Users { get; }
    IRepository<Notification> Notifications { get; }

    /// <summary>Flushes all pending changes. Call once at the end of each command handler.</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
