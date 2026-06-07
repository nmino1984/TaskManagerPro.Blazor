namespace TaskManagerPro.Blazor.Web.Services;

public class AvatarStateService
{
    public string? AvatarUrl { get; private set; }
    public event Action? OnAvatarChanged;

    public void SetAvatar(string? url)
    {
        AvatarUrl = url;
        OnAvatarChanged?.Invoke();
    }
}
