using MediatR;

namespace TaskManagerPro.Blazor.Application.Features.Auth.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<LoginResult>;
