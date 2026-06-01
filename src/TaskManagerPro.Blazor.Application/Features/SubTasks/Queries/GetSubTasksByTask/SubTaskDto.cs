namespace TaskManagerPro.Blazor.Application.Features.SubTasks.Queries.GetSubTasksByTask;

/// <summary>
/// Read model for a subtask. All properties are exposed as they are needed
/// by UI components to render progress within a task.
/// </summary>
public record SubTaskDto(
    Guid Id,
    string Title,
    string Description,
    bool IsCompleted,
    Guid TaskItemId,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
