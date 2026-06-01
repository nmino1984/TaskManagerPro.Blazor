using MediatR;

namespace TaskManagerPro.Blazor.Application.Features.SubTasks.Commands.CreateSubTask;

/// <summary>
/// Command to add a new subtask to an existing task.
/// </summary>
public record CreateSubTaskCommand(
    string Title,
    string Description,
    Guid TaskItemId
) : IRequest<Guid>;
