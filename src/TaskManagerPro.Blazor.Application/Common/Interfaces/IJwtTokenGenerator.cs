namespace TaskManagerPro.Blazor.Application.Common.Interfaces;

/// <summary>
/// Abstracts JWT creation so the Application layer remains independent of
/// the token signing algorithm and secret management strategy.
/// </summary>
public interface IJwtTokenGenerator
{
    /// <summary>
    /// Generates a signed JWT for the given user identity.
    /// Returns the token string and the UTC expiry timestamp.
    /// </summary>
    (string Token, DateTime ExpiresAt) GenerateToken(Guid userId, string email, string firstName, string lastName);
}
