using MediatR;
using Microsoft.Extensions.Logging;
using TaskManagerPro.Blazor.Application.Common.Exceptions;
using TaskManagerPro.Blazor.Domain.Entities;
using TaskManagerPro.Blazor.Domain.Enums;
using TaskManagerPro.Blazor.Domain.Interfaces;

namespace TaskManagerPro.Blazor.Application.Features.Milestones.Commands.UpdateMilestone;

public class UpdateMilestoneCommandHandler : IRequestHandler<UpdateMilestoneCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateMilestoneCommandHandler> _logger;

    public UpdateMilestoneCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateMilestoneCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(UpdateMilestoneCommand request, CancellationToken cancellationToken)
    {
        var milestone = await _unitOfWork.Milestones.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Milestone), request.Id);

        MilestoneStatus previousStatus = milestone.Status;
        milestone.Update(request.Title, request.Description, request.TargetDate, request.Status);

        await _unitOfWork.Milestones.UpdateAsync(milestone, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (previousStatus != MilestoneStatus.Completed && milestone.Status == MilestoneStatus.Completed)
            _logger.LogInformation("Milestone {MilestoneId} completed in Task {TaskId}", milestone.Id, milestone.TaskItemId);
    }
}
