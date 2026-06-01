using MediatR;

namespace TaskManagerPro.Blazor.Application.Features.SubTasks.Queries.GetSubTasksByTask;

/// <summary>
/// Query to retrieve all non-deleted subtasks belonging to a specific task.
/// </summary>
public record GetSubTasksByTaskQuery(Guid TaskItemId) : IRequest<IEnumerable<SubTaskDto>>;
