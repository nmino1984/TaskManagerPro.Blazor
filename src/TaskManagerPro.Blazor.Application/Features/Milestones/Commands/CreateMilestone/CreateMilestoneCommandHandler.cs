using MediatR;
using TaskManagerPro.Blazor.Application.Common.Exceptions;
using TaskManagerPro.Blazor.Domain.Entities;
using TaskManagerPro.Blazor.Domain.Interfaces;

namespace TaskManagerPro.Blazor.Application.Features.Milestones.Commands.CreateMilestone;

/// <summary>
/// Creates a milestone after confirming the parent task exists.
/// </summary>
public class CreateMilestoneCommandHandler : IRequestHandler<CreateMilestoneCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateMilestoneCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateMilestoneCommand request, CancellationToken cancellationToken)
    {
        TaskItem? parent = await _unitOfWork.Tasks.GetByIdAsync(request.TaskItemId, cancellationToken);

        if (parent is null)
            throw new NotFoundException(nameof(TaskItem), request.TaskItemId);

        Milestone milestone = new(request.Title, request.Description, request.TargetDate, request.TaskItemId);

        await _unitOfWork.Milestones.AddAsync(milestone, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return milestone.Id;
    }
}
