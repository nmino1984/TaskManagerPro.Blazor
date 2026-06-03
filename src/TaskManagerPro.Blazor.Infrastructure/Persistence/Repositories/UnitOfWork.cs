using TaskManagerPro.Blazor.Domain.Entities;
using TaskManagerPro.Blazor.Domain.Interfaces;
using TaskManagerPro.Blazor.Infrastructure.Persistence.Context;

namespace TaskManagerPro.Blazor.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repositories are lazily initialised so the DbSet cost is only paid when that
/// aggregate is actually used. All repositories share the same DbContext instance,
/// guaranteeing they participate in the same transaction on SaveChangesAsync.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    private IRepository<TaskItem>?    _tasks;
    private IRepository<SubTask>?     _subTasks;
    private IRepository<Milestone>?   _milestones;
    private IRepository<AppUser>?     _users;
    private IRepository<Notification>? _notifications;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IRepository<TaskItem>     Tasks         => _tasks         ??= new Repository<TaskItem>(_context);
    public IRepository<SubTask>      SubTasks      => _subTasks      ??= new Repository<SubTask>(_context);
    public IRepository<Milestone>    Milestones    => _milestones    ??= new Repository<Milestone>(_context);
    public IRepository<AppUser>      Users         => _users         ??= new Repository<AppUser>(_context);
    public IRepository<Notification> Notifications => _notifications ??= new Repository<Notification>(_context);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);

    public void Dispose()
        => _context.Dispose();
}
