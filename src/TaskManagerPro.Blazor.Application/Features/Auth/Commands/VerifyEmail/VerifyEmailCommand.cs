using MediatR;

namespace TaskManagerPro.Blazor.Application.Features.Auth.Commands.VerifyEmail;

public record VerifyEmailCommand(string Token) : IRequest<VerifyEmailResult>;
