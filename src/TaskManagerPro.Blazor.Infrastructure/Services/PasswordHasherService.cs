using TaskManagerPro.Blazor.Application.Common.Interfaces;

namespace TaskManagerPro.Blazor.Infrastructure.Services;

/// <summary>
/// BCrypt is used because it is adaptive (work factor can increase over time) and
/// embeds the salt inside the hash string, so salts never need to be managed separately.
/// </summary>
public class PasswordHasherService : IPasswordHasher
{
    // 12 balances security and latency (~250ms per hash on modern hardware)
    private const int WorkFactor = 12;

    public string Hash(string password)
        => BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);

    // BCrypt.Verify uses constant-time comparison to prevent timing attacks
    public bool Verify(string password, string hash)
        => BCrypt.Net.BCrypt.Verify(password, hash);
}
