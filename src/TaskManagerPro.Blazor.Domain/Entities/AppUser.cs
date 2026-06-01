using TaskManagerPro.Blazor.Domain.Common;

namespace TaskManagerPro.Blazor.Domain.Entities;

/// <summary>
/// Represents an application user. Owns tasks and receives notifications.
/// Kept separate from ASP.NET Identity to preserve Domain independence.
/// </summary>
public class AppUser : BaseEntity
{
    /// <summary>
    /// User's given name, used for display and personalisation.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// User's family name, used for display and personalisation.
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Unique email address used for login and notifications.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Hashed representation of the user's password. Never stored in plain text.
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// All tasks owned by this user.
    /// </summary>
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();

    /// <summary>
    /// Notifications addressed to this user across all their tasks.
    /// </summary>
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
