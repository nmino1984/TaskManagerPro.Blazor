using MediatR;

namespace TaskManagerPro.Blazor.Application.Features.SubTasks.Commands.DeleteSubTask;

/// <summary>
/// Command to soft-delete a subtask by its identifier.
/// </summary>
public record DeleteSubTaskCommand(Guid Id) : IRequest;
