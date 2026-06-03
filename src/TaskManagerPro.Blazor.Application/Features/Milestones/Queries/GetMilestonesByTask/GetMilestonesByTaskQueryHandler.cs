using MediatR;
using TaskManagerPro.Blazor.Domain.Interfaces;

namespace TaskManagerPro.Blazor.Application.Features.Milestones.Queries.GetMilestonesByTask;

public class GetMilestonesByTaskQueryHandler : IRequestHandler<GetMilestonesByTaskQuery, IEnumerable<MilestoneDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetMilestonesByTaskQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<MilestoneDto>> Handle(GetMilestonesByTaskQuery request, CancellationToken cancellationToken)
    {
        var milestones = await _unitOfWork.Milestones.FindAsync(
            m => m.TaskItemId == request.TaskItemId, cancellationToken);

        return milestones.Select(m => new MilestoneDto(
            m.Id, m.Title, m.Description, m.TargetDate, m.Status, m.TaskItemId, m.CreatedAt, m.UpdatedAt));
    }
}
