using TaskManagerPro.Blazor.Application.Common.Interfaces;

namespace TaskManagerPro.Blazor.Infrastructure.Services;

/// <summary>
/// BCrypt-based implementation of IPasswordHasher. BCrypt is chosen because it
/// is adaptive (work factor can be increased over time) and embeds the salt inside
/// the hash string, eliminating the need to manage salts separately.
/// </summary>
public class PasswordHasherService : IPasswordHasher
{
    /// <summary>
    /// BCrypt work factor. 12 is a reasonable default that balances security
    /// and latency on modern hardware (~250ms per hash at this setting).
    /// </summary>
    private const int WorkFactor = 12;

    /// <summary>
    /// Produces a BCrypt hash of the plain-text password with an embedded random salt.
    /// The work factor is baked into the returned string so Verify() never needs it separately.
    /// </summary>
    public string Hash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);
    }

    /// <summary>
    /// Validates a plain-text password against its stored BCrypt hash.
    /// BCrypt.Verify performs a constant-time comparison to prevent timing attacks.
    /// </summary>
    public bool Verify(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
