using MediatR;
using TaskManagerPro.Blazor.Application.Common.Models;

namespace TaskManagerPro.Blazor.Application.Features.Tasks.Queries.GetAllTasks;

public record GetAllTasksQuery(
    Guid UserId,
    int Page = 1,
    int PageSize = 10
) : IRequest<PagedResult<TaskItemDto>>;
