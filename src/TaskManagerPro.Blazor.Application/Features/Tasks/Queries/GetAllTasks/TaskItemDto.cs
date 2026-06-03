using TaskManagerPro.Blazor.Domain.Enums;

namespace TaskManagerPro.Blazor.Application.Features.Tasks.Queries.GetAllTasks;

public record TaskItemDto(
    Guid Id,
    string Title,
    string Description,
    DateTime? DueDate,
    TaskPriority Priority,
    WorkTaskStatus Status,
    Guid UserId,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    bool IsDeleted
);
