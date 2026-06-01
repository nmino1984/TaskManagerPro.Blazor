using TaskManagerPro.Blazor.Domain.Entities;
using TaskManagerPro.Blazor.Domain.Interfaces;
using TaskManagerPro.Blazor.Infrastructure.Persistence.Context;

namespace TaskManagerPro.Blazor.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of IUnitOfWork. Repositories are lazily initialised
/// so the cost of constructing a DbSet is only paid when that aggregate is actually used
/// within a given request. All repositories share the same DbContext instance,
/// which guarantees they participate in the same transaction when SaveChangesAsync is called.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    private IRepository<TaskItem>? _tasks;
    private IRepository<SubTask>? _subTasks;
    private IRepository<Milestone>? _milestones;
    private IRepository<AppUser>? _users;
    private IRepository<Notification>? _notifications;

    /// <summary>
    /// Initialises the unit of work with the scoped DbContext for the current request.
    /// </summary>
    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public IRepository<TaskItem> Tasks =>
        _tasks ??= new Repository<TaskItem>(_context);

    /// <inheritdoc />
    public IRepository<SubTask> SubTasks =>
        _subTasks ??= new Repository<SubTask>(_context);

    /// <inheritdoc />
    public IRepository<Milestone> Milestones =>
        _milestones ??= new Repository<Milestone>(_context);

    /// <inheritdoc />
    public IRepository<AppUser> Users =>
        _users ??= new Repository<AppUser>(_context);

    /// <inheritdoc />
    public IRepository<Notification> Notifications =>
        _notifications ??= new Repository<Notification>(_context);

    /// <inheritdoc />
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _context.Dispose();
    }
}
