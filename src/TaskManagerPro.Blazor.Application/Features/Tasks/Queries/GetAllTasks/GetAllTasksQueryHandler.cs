using MediatR;
using TaskManagerPro.Blazor.Application.Common.Models;
using TaskManagerPro.Blazor.Domain.Interfaces;

namespace TaskManagerPro.Blazor.Application.Features.Tasks.Queries.GetAllTasks;

/// <summary>
/// Handler for GetAllTasksQuery.
/// </summary>
public class GetAllTasksQueryHandler : IRequestHandler<GetAllTasksQuery, PagedResult<TaskItemDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllTasksQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<TaskItemDto>> Handle(GetAllTasksQuery request, CancellationToken cancellationToken)
    {
        var userTasks = (await _unitOfWork.Tasks.FindAsync(
            t => t.UserId == request.UserId && !t.IsDeleted,
            cancellationToken)).ToList();

        var totalCount = userTasks.Count;
        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        var paginatedTasks = userTasks
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(t => new TaskItemDto(
                t.Id,
                t.Title,
                t.Description,
                t.DueDate,
                t.Priority,
                t.Status,
                t.UserId,
                t.CreatedAt,
                t.UpdatedAt,
                t.IsDeleted
            ))
            .ToList();

        return new PagedResult<TaskItemDto>
        {
            Items = paginatedTasks,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
