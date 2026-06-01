using MediatR;

namespace TaskManagerPro.Blazor.Application.Features.Auth.Commands.Login;

/// <summary>
/// Command to authenticate a user with email and password credentials.
/// </summary>
public record LoginCommand(string Email, string Password) : IRequest<LoginResult>;
