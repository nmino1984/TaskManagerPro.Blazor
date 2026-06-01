using MediatR;

namespace TaskManagerPro.Blazor.Application.Features.SubTasks.Commands.UpdateSubTask;

/// <summary>
/// Command to update an existing subtask's fields and completion state.
/// </summary>
public record UpdateSubTaskCommand(
    Guid Id,
    string Title,
    string Description,
    bool IsCompleted
) : IRequest;
