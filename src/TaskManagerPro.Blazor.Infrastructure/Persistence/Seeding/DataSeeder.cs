using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManagerPro.Blazor.Domain.Entities;
using TaskManagerPro.Blazor.Domain.Enums;
using TaskManagerPro.Blazor.Infrastructure.Identity;
using TaskManagerPro.Blazor.Infrastructure.Persistence.Context;

namespace TaskManagerPro.Blazor.Infrastructure.Persistence.Seeding;

/// <summary>
/// Seeds the database with a demo user and representative task data on first run.
/// Every operation checks for existing records before inserting, making the seeder
/// safe to call on every application startup without creating duplicates.
/// </summary>
public static class DataSeeder
{
    private const string DemoEmail = "demo@taskmanager.com";
    private const string DemoPassword = "Demo1234!";

    /// <summary>
    /// Entry point called from Program.cs after EnsureCreated / migrations.
    /// Creates the demo Identity user, the matching Domain AppUser, and three
    /// sample tasks (each with subtasks and milestones) covering all priority levels.
    /// </summary>
    public static async Task SeedAsync(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        await SeedIdentityUserAsync(userManager);
        Guid appUserId = await SeedAppUserAsync(context);
        await SeedTasksAsync(context, appUserId);
    }

    // -------------------------------------------------------------------------
    // Identity user
    // -------------------------------------------------------------------------

    /// <summary>
    /// Creates the ASP.NET Identity record for the demo account if it does not exist.
    /// </summary>
    private static async Task SeedIdentityUserAsync(UserManager<ApplicationUser> userManager)
    {
        bool exists = await userManager.Users.AnyAsync(u => u.Email == DemoEmail);
        if (exists) return;

        ApplicationUser identityUser = new()
        {
            UserName = DemoEmail,
            Email = DemoEmail,
            FirstName = "Demo",
            LastName = "User",
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow
        };

        await userManager.CreateAsync(identityUser, DemoPassword);
    }

    // -------------------------------------------------------------------------
    // Domain AppUser
    // -------------------------------------------------------------------------

    /// <summary>
    /// Creates the Domain AppUser record for the demo account if it does not exist.
    /// Returns the Guid used as the foreign key on task records.
    /// </summary>
    private static async Task<Guid> SeedAppUserAsync(ApplicationDbContext context)
    {
        AppUser? existing = await context.AppUsers
            .FirstOrDefaultAsync(u => u.Email == DemoEmail);

        if (existing is not null) return existing.Id;

        string passwordHash = BCrypt.Net.BCrypt.HashPassword(DemoPassword);
        AppUser appUser = new("Demo", "User", DemoEmail, passwordHash);

        await context.AppUsers.AddAsync(appUser);
        await context.SaveChangesAsync();

        return appUser.Id;
    }

    // -------------------------------------------------------------------------
    // Tasks, SubTasks, Milestones
    // -------------------------------------------------------------------------

    /// <summary>
    /// Creates three representative tasks with subtasks and milestones.
    /// Skips seeding entirely if any task already exists for the demo user.
    /// </summary>
    private static async Task SeedTasksAsync(ApplicationDbContext context, Guid appUserId)
    {
        bool tasksExist = await context.Tasks.AnyAsync(t => t.UserId == appUserId);
        if (tasksExist) return;

        TaskItem devTask = await CreateTaskAsync(
            context,
            appUserId,
            "Set up development environment",
            "Configure local development tools, SDKs, Docker, and the SQL Server container.",
            DateTime.UtcNow.AddDays(7),
            TaskPriority.Low,
            WorkTaskStatus.InProgress);

        await context.SubTasks.AddAsync(new SubTask("Install .NET 10 SDK", "Download and verify dotnet --version", devTask.Id));
        await context.SubTasks.AddAsync(new SubTask("Run SQL Server in Docker", "Use docker run with SA_PASSWORD and port 1433", devTask.Id));
        await context.SubTasks.AddAsync(new SubTask("Apply EF Core migrations", "Run dotnet ef database update from Infrastructure project", devTask.Id));
        await context.Milestones.AddAsync(new Milestone("Environment ready", "All tools installed and database running", DateTime.UtcNow.AddDays(3), devTask.Id));

        TaskItem dbTask = await CreateTaskAsync(
            context,
            appUserId,
            "Design database schema",
            "Define all entity tables, relationships, indexes, and EF Core configurations.",
            DateTime.UtcNow.AddDays(-2),
            TaskPriority.High,
            WorkTaskStatus.Completed);

        await context.SubTasks.AddAsync(new SubTask("Define Domain entities", "BaseEntity, TaskItem, SubTask, Milestone, Notification, AppUser", dbTask.Id));
        await context.SubTasks.AddAsync(new SubTask("Write EF Core configurations", "One IEntityTypeConfiguration per aggregate", dbTask.Id));
        await context.SubTasks.AddAsync(new SubTask("Create initial migration", "dotnet ef migrations add InitialCreate", dbTask.Id));
        await context.Milestones.AddAsync(new Milestone("Schema approved", "All configurations reviewed and migration generated", DateTime.UtcNow.AddDays(-5), dbTask.Id));
        await context.Milestones.AddAsync(new Milestone("Migration applied to staging", "Database reflects final schema on staging environment", DateTime.UtcNow.AddDays(-1), dbTask.Id));

        TaskItem authTask = await CreateTaskAsync(
            context,
            appUserId,
            "Implement authentication",
            "Build register and login flows using ASP.NET Identity, BCrypt, and JWT tokens.",
            DateTime.UtcNow.AddDays(14),
            TaskPriority.Critical,
            WorkTaskStatus.Pending);

        await context.SubTasks.AddAsync(new SubTask("Implement RegisterCommand handler", "Hash password, check duplicate email, persist AppUser", authTask.Id));
        await context.SubTasks.AddAsync(new SubTask("Implement LoginCommand handler", "Verify credentials, issue signed JWT", authTask.Id));
        await context.SubTasks.AddAsync(new SubTask("Protect Blazor routes", "Add [Authorize] attributes and configure auth middleware", authTask.Id));
        await context.Milestones.AddAsync(new Milestone("Register flow working", "New user can create an account and receive a token", DateTime.UtcNow.AddDays(7), authTask.Id));
        await context.Milestones.AddAsync(new Milestone("Login flow working", "Existing user can log in and access protected pages", DateTime.UtcNow.AddDays(14), authTask.Id));

        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Creates a TaskItem, adds it to the context, saves to obtain its database-assigned Id,
    /// then sets the desired status via EF Core's change-tracker metadata so the
    /// backing field is written without exposing a public setter on the domain entity.
    /// </summary>
    private static async Task<TaskItem> CreateTaskAsync(
        ApplicationDbContext context,
        Guid userId,
        string title,
        string description,
        DateTime dueDate,
        TaskPriority priority,
        WorkTaskStatus status)
    {
        TaskItem task = new(title, description, dueDate, priority, userId);
        await context.Tasks.AddAsync(task);
        await context.SaveChangesAsync();

        if (status != WorkTaskStatus.Pending)
        {
            context.Entry(task).Property(nameof(TaskItem.Status)).CurrentValue = status;
            await context.SaveChangesAsync();
        }

        return task;
    }
}
