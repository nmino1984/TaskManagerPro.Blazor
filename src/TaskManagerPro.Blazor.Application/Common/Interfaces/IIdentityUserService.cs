namespace TaskManagerPro.Blazor.Application.Common.Interfaces;

/// <summary>
/// Abstracts Identity-side user mutations so Application handlers stay decoupled from UserManager.
/// </summary>
public interface IIdentityUserService
{
    Task UpdateUserAsync(Guid userId, string email, string firstName, string lastName, CancellationToken ct);

    /// <summary>Replaces the user's Identity password without requiring the old password (caller must verify first).</summary>
    Task ResetPasswordAsync(Guid userId, string newPassword, CancellationToken ct);
}
