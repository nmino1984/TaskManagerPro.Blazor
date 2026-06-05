using MediatR;
using TaskManagerPro.Blazor.Application.Common.Models;
using TaskManagerPro.Blazor.Domain.Interfaces;

namespace TaskManagerPro.Blazor.Application.Features.Tasks.Queries.GetAllTasks;

public class GetAllTasksQueryHandler : IRequestHandler<GetAllTasksQuery, PagedResult<TaskItemDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllTasksQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<TaskItemDto>> Handle(GetAllTasksQuery request, CancellationToken cancellationToken)
    {
        var tasks = (await _unitOfWork.Tasks.FindAsync(
            t => (t.UserId == request.UserId || t.AssignedToUserId == request.UserId) && !t.IsDeleted,
            cancellationToken)).ToList();

        var items = tasks
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(t => new TaskItemDto(t.Id, t.Title, t.Description, t.DueDate, t.Priority,
                                         t.Status, t.UserId, t.CreatedAt, t.UpdatedAt, t.IsDeleted,
                                         t.AssignedToUserId))
            .ToList();

        return new PagedResult<TaskItemDto>
        {
            Items = items,
            TotalCount = tasks.Count,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
