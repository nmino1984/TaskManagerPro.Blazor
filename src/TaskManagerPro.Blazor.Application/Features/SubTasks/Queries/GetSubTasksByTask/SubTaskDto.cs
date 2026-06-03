namespace TaskManagerPro.Blazor.Application.Features.SubTasks.Queries.GetSubTasksByTask;

public record SubTaskDto(
    Guid Id,
    string Title,
    string Description,
    bool IsCompleted,
    Guid TaskItemId,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
