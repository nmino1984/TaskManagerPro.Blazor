using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskManagerPro.Blazor.Domain.Common;
using TaskManagerPro.Blazor.Domain.Entities;
using TaskManagerPro.Blazor.Infrastructure.Identity;
using TaskManagerPro.Blazor.Infrastructure.Persistence.Configurations;

namespace TaskManagerPro.Blazor.Infrastructure.Persistence.Context;

/// <summary>
/// Single DbContext for the application. Inherits IdentityDbContext so Identity
/// and Domain tables share the same database and transaction scope.
/// UpdatedAt is stamped automatically on every save via the change tracker —
/// command handlers never need to manage the audit timestamp manually.
/// </summary>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<SubTask> SubTasks => Set<SubTask>();
    public DbSet<Milestone> Milestones => Set<Milestone>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<AppUser> AppUsers => Set<AppUser>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfiguration(new TaskItemConfiguration());
        builder.ApplyConfiguration(new SubTaskConfiguration());
        builder.ApplyConfiguration(new MilestoneConfiguration());
        builder.ApplyConfiguration(new NotificationConfiguration());
        builder.ApplyConfiguration(new AppUserConfiguration());

        // Tell EF Core to read/write UpdatedAt via the _updatedAt backing field
        // so the domain property can stay getter-only without a public setter
        foreach (var entityType in builder.Model.GetEntityTypes()
            .Where(e => typeof(BaseEntity).IsAssignableFrom(e.ClrType)))
        {
            builder.Entity(entityType.ClrType)
                .Property(nameof(BaseEntity.UpdatedAt))
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Modified)
                entry.Property(nameof(BaseEntity.UpdatedAt)).CurrentValue = now;
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
