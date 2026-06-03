using Microsoft.AspNetCore.Identity;

namespace TaskManagerPro.Blazor.Infrastructure.Identity;

/// <summary>
/// ASP.NET Identity user record. Kept separate from Domain.Entities.AppUser
/// so the Domain layer has zero dependency on Identity infrastructure.
/// Registration creates both records; they are linked by email address.
/// </summary>
public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    // Stored on the Identity record so registration-date filters don't require a join to the Domain table
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Soft-delete — deactivates the account without removing credentials or audit history
    public bool IsDeleted { get; set; } = false;
}
