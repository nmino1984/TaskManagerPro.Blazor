using Microsoft.AspNetCore.Identity;

namespace TaskManagerPro.Blazor.Infrastructure.Identity;

/// <summary>
/// ASP.NET Identity user record. Kept separate from <c>Domain.Entities.AppUser</c>
/// so the Domain layer has zero dependency on Identity infrastructure.
/// A synchronisation step in the registration flow creates a matching AppUser
/// after this record is persisted, maintaining a clean boundary between layers.
/// </summary>
public class ApplicationUser : IdentityUser
{
    /// <summary>User's given name, mirrored from the Domain AppUser.</summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>User's family name, mirrored from the Domain AppUser.</summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// UTC timestamp recorded on account creation.
    /// Stored here so Identity queries can filter by registration date
    /// without joining to the Domain table.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Soft-delete flag. Allows account deactivation without permanently
    /// removing identity credentials or audit history.
    /// </summary>
    public bool IsDeleted { get; set; } = false;
}
