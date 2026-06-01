using MediatR;
using TaskManagerPro.Blazor.Application.Common.Exceptions;
using TaskManagerPro.Blazor.Domain.Entities;
using TaskManagerPro.Blazor.Domain.Interfaces;

namespace TaskManagerPro.Blazor.Application.Features.Milestones.Commands.UpdateMilestone;

/// <summary>
/// Applies field updates to a milestone via the domain Update() method.
/// </summary>
public class UpdateMilestoneCommandHandler : IRequestHandler<UpdateMilestoneCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateMilestoneCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateMilestoneCommand request, CancellationToken cancellationToken)
    {
        Milestone? milestone = await _unitOfWork.Milestones.GetByIdAsync(request.Id, cancellationToken);

        if (milestone is null)
            throw new NotFoundException(nameof(Milestone), request.Id);

        milestone.Update(request.Title, request.Description, request.TargetDate, request.Status);

        await _unitOfWork.Milestones.UpdateAsync(milestone, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
