using TaskManagerPro.Blazor.Domain.Common;

namespace TaskManagerPro.Blazor.Domain.Entities;

public class AppUser : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    // Navigation properties
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
