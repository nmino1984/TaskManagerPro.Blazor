using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TaskManagerPro.Blazor.Domain.Entities;
using TaskManagerPro.Blazor.Domain.Enums;
using TaskManagerPro.Blazor.Infrastructure.Persistence.Context;

namespace TaskManagerPro.Blazor.Infrastructure.BackgroundJobs;

public class OverdueNotificationJob : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OverdueNotificationJob> _logger;

    public OverdueNotificationJob(IServiceScopeFactory scopeFactory, ILogger<OverdueNotificationJob> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await RunAsync(stoppingToken);
            try
            {
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }

    private async Task RunAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var overdueTasks = await db.Tasks
                .Where(t => t.DueDate < DateTime.UtcNow
                         && !t.OverdueNotified
                         && t.Status != WorkTaskStatus.Completed
                         && t.Status != WorkTaskStatus.Cancelled
                         && !t.IsDeleted)
                .ToListAsync(cancellationToken);

            foreach (var task in overdueTasks)
            {
                var userId = task.AssignedToUserId ?? task.UserId;
                var notification = new Notification(
                    "Task overdue",
                    $"'{task.Title}' was due on {task.DueDate:MMM dd, yyyy}",
                    userId,
                    task.Id);

                await db.Notifications.AddAsync(notification, cancellationToken);
                task.MarkOverdueNotified();

                _logger.LogWarning("Overdue notification sent for task {TaskId}", task.Id);
            }

            if (overdueTasks.Count > 0)
            {
                await db.SaveChangesAsync(cancellationToken);
            }
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error running overdue notification job");
        }
    }
}
