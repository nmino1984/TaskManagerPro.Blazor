using MediatR;

namespace TaskManagerPro.Blazor.Application.Features.Tasks.Commands.DeleteTask;

/// <summary>
/// Command to soft-delete a task. UserId is required so the handler can verify
/// that only the owner can delete their own task.
/// </summary>
public record DeleteTaskCommand(Guid Id, Guid UserId) : IRequest;
