using TaskManagerPro.Blazor.Domain.Entities;

namespace TaskManagerPro.Blazor.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<TaskItem> Tasks { get; }
    IRepository<SubTask> SubTasks { get; }
    IRepository<Milestone> Milestones { get; }
    IRepository<AppUser> Users { get; }
    IRepository<Notification> Notifications { get; }

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
