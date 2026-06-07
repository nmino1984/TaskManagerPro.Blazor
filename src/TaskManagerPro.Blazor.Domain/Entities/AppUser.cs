using TaskManagerPro.Blazor.Domain.Common;

namespace TaskManagerPro.Blazor.Domain.Entities;

// Kept separate from ASP.NET Identity to avoid pulling infra deps into Domain
public class AppUser : BaseEntity
{
    // Required by EF Core — not for direct use
    private AppUser() { }

    /// <summary>
    /// PasswordHash must already be hashed by the caller (Infrastructure's IPasswordHasher).
    /// Plain text is never accepted here.
    /// </summary>
    public AppUser(string firstName, string lastName, string email, string passwordHash)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PasswordHash = passwordHash;
    }

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    private bool _isEmailVerified = false;
    public bool IsEmailVerified => _isEmailVerified;

    private string? _avatarUrl;
    public string? AvatarUrl => _avatarUrl;

    private string? _verificationToken;
    public string? VerificationToken => _verificationToken;

    private DateTime? _verificationTokenExpiry;
    public DateTime? VerificationTokenExpiry => _verificationTokenExpiry;

    public void SetAvatarUrl(string url)
    {
        _avatarUrl = url;
    }

    public void SetVerificationToken(string token, DateTime expiry)
    {
        _verificationToken = token;
        _verificationTokenExpiry = expiry;
        _isEmailVerified = false;
    }

    public void VerifyEmail()
    {
        _isEmailVerified = true;
        _verificationToken = null;
        _verificationTokenExpiry = null;
    }

    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
