using MediatR;
using TaskManagerPro.Blazor.Application.Common.Exceptions;
using TaskManagerPro.Blazor.Domain.Entities;
using TaskManagerPro.Blazor.Domain.Interfaces;

namespace TaskManagerPro.Blazor.Application.Features.Milestones.Commands.DeleteMilestone;

public class DeleteMilestoneCommandHandler : IRequestHandler<DeleteMilestoneCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteMilestoneCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteMilestoneCommand request, CancellationToken cancellationToken)
    {
        var milestone = await _unitOfWork.Milestones.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Milestone), request.Id);

        await _unitOfWork.Milestones.DeleteAsync(milestone, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
