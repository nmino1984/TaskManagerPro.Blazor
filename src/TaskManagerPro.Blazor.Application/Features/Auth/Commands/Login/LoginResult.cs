namespace TaskManagerPro.Blazor.Application.Features.Auth.Commands.Login;

/// <summary>
/// Carries the JWT and user identity returned after a successful login.
/// ExpiresAt lets the client schedule a proactive token refresh.
/// </summary>
public record LoginResult(
    string Token,
    Guid UserId,
    string Email,
    DateTime ExpiresAt
);
