using MediatR;
using TaskManagerPro.Blazor.Domain.Enums;

namespace TaskManagerPro.Blazor.Application.Features.Tasks.Commands.UpdateTask;

/// <summary>
/// Status is optional — null means leave the current status unchanged.
/// </summary>
public record UpdateTaskCommand(
    Guid Id,
    string Title,
    string Description,
    DateTime? DueDate,
    TaskPriority Priority,
    WorkTaskStatus? Status = null
) : IRequest;
