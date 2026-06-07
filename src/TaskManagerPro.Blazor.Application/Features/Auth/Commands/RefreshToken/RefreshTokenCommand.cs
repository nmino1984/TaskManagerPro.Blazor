using MediatR;

namespace TaskManagerPro.Blazor.Application.Features.Auth.Commands.RefreshToken;

public record RefreshTokenCommand(Guid UserId) : IRequest<string>;
