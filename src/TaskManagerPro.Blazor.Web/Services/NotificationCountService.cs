namespace TaskManagerPro.Blazor.Web.Services;

/// <summary>
/// Scoped per-circuit service that lets Notifications.razor push unread count
/// updates to MainLayout without requiring a full page reload.
/// </summary>
public class NotificationCountService
{
    public int UnreadCount { get; private set; }
    public event Action? OnCountChanged;

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
}
