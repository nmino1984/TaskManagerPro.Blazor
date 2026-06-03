using MediatR;

namespace TaskManagerPro.Blazor.Application.Features.Tasks.Commands.DeleteTask;

// UserId required so the handler can verify only the owner can delete their task
public record DeleteTaskCommand(Guid Id, Guid UserId) : IRequest;
