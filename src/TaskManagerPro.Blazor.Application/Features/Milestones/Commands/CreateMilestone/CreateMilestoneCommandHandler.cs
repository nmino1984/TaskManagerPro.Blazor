using MediatR;
using TaskManagerPro.Blazor.Application.Common.Exceptions;
using TaskManagerPro.Blazor.Domain.Entities;
using TaskManagerPro.Blazor.Domain.Interfaces;

namespace TaskManagerPro.Blazor.Application.Features.Milestones.Commands.CreateMilestone;

// Verifies the parent task exists to prevent orphaned milestones
public class CreateMilestoneCommandHandler : IRequestHandler<CreateMilestoneCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateMilestoneCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateMilestoneCommand request, CancellationToken cancellationToken)
    {
        _ = await _unitOfWork.Tasks.GetByIdAsync(request.TaskItemId, cancellationToken)
            ?? throw new NotFoundException(nameof(TaskItem), request.TaskItemId);

        var milestone = new Milestone(request.Title, request.Description, request.TargetDate, request.TaskItemId);

        await _unitOfWork.Milestones.AddAsync(milestone, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return milestone.Id;
    }
}
