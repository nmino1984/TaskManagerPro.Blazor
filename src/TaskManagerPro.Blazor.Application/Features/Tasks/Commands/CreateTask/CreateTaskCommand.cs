using MediatR;
using TaskManagerPro.Blazor.Domain.Enums;

namespace TaskManagerPro.Blazor.Application.Features.Tasks.Commands.CreateTask;

public record CreateTaskCommand(
    string Title,
    string Description,
    DateTime? DueDate,
    TaskPriority Priority,
    Guid UserId,
    Guid? AssignedToUserId = null
) : IRequest<Guid>;
