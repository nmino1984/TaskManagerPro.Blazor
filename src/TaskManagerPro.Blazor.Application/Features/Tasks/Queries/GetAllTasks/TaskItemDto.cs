using TaskManagerPro.Blazor.Domain.Enums;

namespace TaskManagerPro.Blazor.Application.Features.Tasks.Queries.GetAllTasks;

/// <summary>
/// Data Transfer Object for TaskItem.
/// </summary>
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
