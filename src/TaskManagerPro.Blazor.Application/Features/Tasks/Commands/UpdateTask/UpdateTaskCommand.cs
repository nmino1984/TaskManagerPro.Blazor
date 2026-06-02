using MediatR;
using TaskManagerPro.Blazor.Domain.Enums;

namespace TaskManagerPro.Blazor.Application.Features.Tasks.Commands.UpdateTask;

/// <summary>
/// Command to update the mutable fields of an existing task, including an optional
/// status transition. When Status is null the current status is left unchanged.
/// </summary>
public record UpdateTaskCommand(
    Guid Id,
    string Title,
    string Description,
    DateTime? DueDate,
    TaskPriority Priority,
    WorkTaskStatus? Status = null
) : IRequest;
