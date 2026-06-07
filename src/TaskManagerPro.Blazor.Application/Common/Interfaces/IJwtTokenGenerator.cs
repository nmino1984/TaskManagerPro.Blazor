namespace TaskManagerPro.Blazor.Application.Common.Interfaces;

/// <summary>
/// Abstracts JWT creation so the Application layer stays independent of
/// the signing algorithm and secret management strategy used in Infrastructure.
/// </summary>
public interface IJwtTokenGenerator
{
    /// <summary>Returns the signed token string and its UTC expiry timestamp.</summary>
    (string Token, DateTime ExpiresAt) GenerateToken(Guid userId, string email, string firstName, string lastName, bool isEmailVerified);
}
