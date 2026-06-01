using MediatR;
using TaskManagerPro.Blazor.Domain.Interfaces;

namespace TaskManagerPro.Blazor.Application.Features.SubTasks.Queries.GetSubTasksByTask;

/// <summary>
/// Fetches subtasks for a task using FindAsync so the TaskItemId filter
/// is evaluated in the database rather than in memory.
/// </summary>
public class GetSubTasksByTaskQueryHandler : IRequestHandler<GetSubTasksByTaskQuery, IEnumerable<SubTaskDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetSubTasksByTaskQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<SubTaskDto>> Handle(GetSubTasksByTaskQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Domain.Entities.SubTask> subTasks = await _unitOfWork.SubTasks.FindAsync(
            s => s.TaskItemId == request.TaskItemId, cancellationToken);

        return subTasks.Select(s => new SubTaskDto(
            s.Id,
            s.Title,
            s.Description,
            s.IsCompleted,
            s.TaskItemId,
            s.CreatedAt,
            s.UpdatedAt
        ));
    }
}
