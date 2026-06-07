using MediatR;

namespace TaskManagerPro.Blazor.Application.Features.Auth.Commands.ResendVerificationEmail;

public record ResendVerificationEmailCommand(string Email) : IRequest<ResendVerificationEmailResult>;
