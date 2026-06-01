using MediatR;
using TaskManagerPro.Blazor.Domain.Enums;

namespace TaskManagerPro.Blazor.Application.Features.Tasks.Commands.CreateTask;

/// <summary>
/// Command to create a new task.
/// </summary>
public record CreateTaskCommand(
    string Title,
    string Description,
    DateTime? DueDate,
    TaskPriority Priority,
    Guid UserId
) : IRequest<Guid>;
