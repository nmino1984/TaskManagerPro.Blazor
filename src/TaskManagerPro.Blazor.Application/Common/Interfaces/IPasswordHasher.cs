namespace TaskManagerPro.Blazor.Application.Common.Interfaces;

/// <summary>
/// Abstracts password hashing so the Application layer never depends on a
/// specific hashing algorithm. The implementation lives in Infrastructure.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>Produces a one-way hash of the given plain-text password.</summary>
    string Hash(string password);

    /// <summary>
    /// Returns true when the plain-text password matches the stored hash.
    /// Constant-time comparison is the responsibility of the implementation.
    /// </summary>
    bool Verify(string password, string hash);
}
