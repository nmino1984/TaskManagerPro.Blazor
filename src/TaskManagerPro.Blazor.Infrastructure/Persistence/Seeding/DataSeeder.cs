using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManagerPro.Blazor.Domain.Entities;
using TaskManagerPro.Blazor.Domain.Enums;
using TaskManagerPro.Blazor.Infrastructure.Identity;
using TaskManagerPro.Blazor.Infrastructure.Persistence.Context;

namespace TaskManagerPro.Blazor.Infrastructure.Persistence.Seeding;

/// <summary>
/// Seeds the database with a demo user and representative task data on first run.
/// All operations check for existing records first, so it is safe to call on every startup.
/// </summary>
public static class DataSeeder
{
    private const string DemoEmail    = "demo@taskmanager.com";
    private const string DemoPassword = "Demo1234!";

    public static async Task SeedAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        await SeedIdentityUserAsync(userManager);
        var appUserId = await SeedAppUserAsync(context);
        await SeedTasksAsync(context, appUserId);
    }

    private static async Task SeedIdentityUserAsync(UserManager<ApplicationUser> userManager)
    {
        if (await userManager.Users.AnyAsync(u => u.Email == DemoEmail)) return;

        var user = new ApplicationUser
        {
            UserName       = DemoEmail,
            Email          = DemoEmail,
            FirstName      = "Demo",
            LastName       = "User",
            EmailConfirmed = true,
            CreatedAt      = DateTime.UtcNow
        };

        await userManager.CreateAsync(user, DemoPassword);
    }

    // Returns the AppUser Id used as FK on task records
    private static async Task<Guid> SeedAppUserAsync(ApplicationDbContext context)
    {
        var existing = await context.AppUsers.FirstOrDefaultAsync(u => u.Email == DemoEmail);
        if (existing is not null) return existing.Id;

        var appUser = new AppUser("Demo", "User", DemoEmail, BCrypt.Net.BCrypt.HashPassword(DemoPassword));
        await context.AppUsers.AddAsync(appUser);
        await context.SaveChangesAsync();
        return appUser.Id;
    }

    private static async Task SeedTasksAsync(ApplicationDbContext context, Guid appUserId)
    {
        if (await context.Tasks.AnyAsync(t => t.UserId == appUserId)) return;

        var devTask = await CreateTaskAsync(context, appUserId,
            "Set up development environment",
            "Configure local development tools, SDKs, Docker, and the SQL Server container.",
            DateTime.UtcNow.AddDays(7), TaskPriority.Low, WorkTaskStatus.InProgress);

        await context.SubTasks.AddAsync(new SubTask("Install .NET 10 SDK", "Download and verify dotnet --version", devTask.Id));
        await context.SubTasks.AddAsync(new SubTask("Run SQL Server in Docker", "Use docker run with SA_PASSWORD and port 1433", devTask.Id));
        await context.SubTasks.AddAsync(new SubTask("Apply EF Core migrations", "Run dotnet ef database update from Infrastructure project", devTask.Id));
        await context.Milestones.AddAsync(new Milestone("Environment ready", "All tools installed and database running", DateTime.UtcNow.AddDays(3), devTask.Id));

        var dbTask = await CreateTaskAsync(context, appUserId,
            "Design database schema",
            "Define all entity tables, relationships, indexes, and EF Core configurations.",
            DateTime.UtcNow.AddDays(-2), TaskPriority.High, WorkTaskStatus.Completed);

        await context.SubTasks.AddAsync(new SubTask("Define Domain entities", "BaseEntity, TaskItem, SubTask, Milestone, Notification, AppUser", dbTask.Id));
        await context.SubTasks.AddAsync(new SubTask("Write EF Core configurations", "One IEntityTypeConfiguration per aggregate", dbTask.Id));
        await context.SubTasks.AddAsync(new SubTask("Create initial migration", "dotnet ef migrations add InitialCreate", dbTask.Id));
        await context.Milestones.AddAsync(new Milestone("Schema approved", "All configurations reviewed and migration generated", DateTime.UtcNow.AddDays(-5), dbTask.Id));
        await context.Milestones.AddAsync(new Milestone("Migration applied to staging", "Database reflects final schema on staging environment", DateTime.UtcNow.AddDays(-1), dbTask.Id));

        var authTask = await CreateTaskAsync(context, appUserId,
            "Implement authentication",
            "Build register and login flows using ASP.NET Identity, BCrypt, and JWT tokens.",
            DateTime.UtcNow.AddDays(14), TaskPriority.Critical, WorkTaskStatus.Pending);

        await context.SubTasks.AddAsync(new SubTask("Implement RegisterCommand handler", "Hash password, check duplicate email, persist AppUser", authTask.Id));
        await context.SubTasks.AddAsync(new SubTask("Implement LoginCommand handler", "Verify credentials, issue signed JWT", authTask.Id));
        await context.SubTasks.AddAsync(new SubTask("Protect Blazor routes", "Add [Authorize] attributes and configure auth middleware", authTask.Id));
        await context.Milestones.AddAsync(new Milestone("Register flow working", "New user can create an account and receive a token", DateTime.UtcNow.AddDays(7), authTask.Id));
        await context.Milestones.AddAsync(new Milestone("Login flow working", "Existing user can log in and access protected pages", DateTime.UtcNow.AddDays(14), authTask.Id));

        await context.SaveChangesAsync();
    }

    // Domain methods are used (not direct field assignment) so business rules are always enforced
    private static async Task<TaskItem> CreateTaskAsync(
        ApplicationDbContext context, Guid userId,
        string title, string description, DateTime dueDate,
        TaskPriority priority, WorkTaskStatus status)
    {
        var task = new TaskItem(title, description, dueDate, priority, userId);

        if (status == WorkTaskStatus.InProgress) task.StartProgress();
        else if (status == WorkTaskStatus.Completed)  task.Complete();
        else if (status == WorkTaskStatus.Cancelled)  task.Cancel();

        await context.Tasks.AddAsync(task);
        await context.SaveChangesAsync();
        return task;
    }
}
