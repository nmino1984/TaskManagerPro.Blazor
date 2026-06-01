using MediatR;
using TaskManagerPro.Blazor.Domain.Enums;

namespace TaskManagerPro.Blazor.Application.Features.Tasks.Commands.UpdateTask;

/// <summary>
/// Command to update the mutable fields of an existing task.
/// Status transitions are intentionally excluded — they belong in dedicated commands.
/// </summary>
public record UpdateTaskCommand(
    Guid Id,
    string Title,
    string Description,
    DateTime? DueDate,
    TaskPriority Priority
) : IRequest;
