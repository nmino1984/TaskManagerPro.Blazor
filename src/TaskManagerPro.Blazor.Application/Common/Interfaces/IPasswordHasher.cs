namespace TaskManagerPro.Blazor.Application.Common.Interfaces;

/// <summary>
/// Abstracts password hashing so the Application layer never depends on a
/// specific algorithm. The implementation lives in Infrastructure.
/// </summary>
public interface IPasswordHasher
{
    string Hash(string password);

    /// <summary>Constant-time comparison is the responsibility of the implementation.</summary>
    bool Verify(string password, string hash);
}
