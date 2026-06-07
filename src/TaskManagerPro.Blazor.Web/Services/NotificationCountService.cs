using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TaskManagerPro.Blazor.Application.Features.Notifications.Queries.GetNotifications;
using TaskManagerPro.Blazor.Domain.Enums;

namespace TaskManagerPro.Blazor.Web.Services;

/// <summary>
/// Scoped per-circuit service that lets Notifications.razor push unread count
/// updates to MainLayout without requiring a full page reload.
/// Call Initialize(userId) once after authentication to start background polling.
/// </summary>
public class NotificationCountService : IDisposable
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<NotificationCountService> _logger;
    private Timer? _timer;
    private Guid _userId;

    public int UnreadCount { get; private set; }
    public event Action? OnCountChanged;

    public NotificationCountService(IServiceScopeFactory scopeFactory, ILogger<NotificationCountService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public void Initialize(Guid userId)
    {
        if (_userId != Guid.Empty) return;
        _userId = userId;
        _timer = new Timer(async _ => await TickAsync(), null, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));
    }

    public void SetCount(int count)
    {
        UnreadCount = count;
        OnCountChanged?.Invoke();
    }

    public void Increment()
    {
        UnreadCount++;
        OnCountChanged?.Invoke();
    }

    private async Task TickAsync()
    {
        _logger.LogInformation("Notification poll tick fired for user {UserId}", _userId);
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var notifications = await mediator.Send(new GetNotificationsQuery(_userId));
            int newCount = notifications.Count(n => n.Status == NotificationStatus.Unread);
            _logger.LogInformation("Tick: newCount={N}, cached={C}", newCount, UnreadCount);
            if (newCount != UnreadCount)
            {
                UnreadCount = newCount;
                OnCountChanged?.Invoke();
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Notification poll tick failed for user {UserId}", _userId);
        }
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
