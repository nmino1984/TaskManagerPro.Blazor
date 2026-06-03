namespace TaskManagerPro.Blazor.Application.Features.Auth.Commands.Login;

// ExpiresAt lets the client schedule a proactive token refresh
public record LoginResult(
    string Token,
    Guid UserId,
    string Email,
    DateTime ExpiresAt
);
