namespace TaskManagerPro.Blazor.Application.Common.Interfaces;

/// <summary>
/// Creates an ASP.NET Identity user during registration.
/// Kept in Application so handlers stay decoupled from the Identity infrastructure.
/// </summary>
public interface IUserRegistrationService
{
    /// <summary>
    /// Creates the ApplicationUser in AspNetUsers with the given id so both records share the same Guid.
    /// Throws if Identity validation fails (duplicate email, weak password, etc.).
    /// </summary>
    Task CreateIdentityUserAsync(Guid userId, string firstName, string lastName, string email, string password, CancellationToken ct);
}
