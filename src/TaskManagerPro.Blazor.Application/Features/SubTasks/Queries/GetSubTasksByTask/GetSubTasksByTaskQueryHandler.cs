using MediatR;
using TaskManagerPro.Blazor.Domain.Interfaces;

namespace TaskManagerPro.Blazor.Application.Features.SubTasks.Queries.GetSubTasksByTask;

public class GetSubTasksByTaskQueryHandler : IRequestHandler<GetSubTasksByTaskQuery, IEnumerable<SubTaskDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetSubTasksByTaskQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<SubTaskDto>> Handle(GetSubTasksByTaskQuery request, CancellationToken cancellationToken)
    {
        var subtasks = await _unitOfWork.SubTasks.FindAsync(
            s => s.TaskItemId == request.TaskItemId, cancellationToken);

        return subtasks.Select(s => new SubTaskDto(
            s.Id, s.Title, s.Description, s.IsCompleted, s.TaskItemId, s.CreatedAt, s.UpdatedAt));
    }
}
