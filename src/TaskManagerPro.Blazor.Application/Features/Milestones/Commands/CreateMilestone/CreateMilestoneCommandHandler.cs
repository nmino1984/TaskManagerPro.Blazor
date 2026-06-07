using MediatR;
using TaskManagerPro.Blazor.Application.Common.Exceptions;
using TaskManagerPro.Blazor.Domain.Entities;
using TaskManagerPro.Blazor.Domain.Interfaces;

namespace TaskManagerPro.Blazor.Application.Features.Milestones.Commands.CreateMilestone;

public class CreateMilestoneCommandHandler : IRequestHandler<CreateMilestoneCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateMilestoneCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateMilestoneCommand request, CancellationToken cancellationToken)
    {
        var task = await _unitOfWork.Tasks.GetByIdAsync(request.TaskItemId, cancellationToken);
        if (task is null)
            throw new NotFoundException(nameof(TaskItem), request.TaskItemId);

        var milestone = new Milestone(request.Title, request.Description, request.TargetDate, request.TaskItemId);
        await _unitOfWork.Milestones.AddAsync(milestone, cancellationToken);

        var notification = new Notification(
            $"Milestone added: '{request.Title}' for task '{task.Title}'",
            $"A new milestone has been added to task '{task.Title}'.",
            task.UserId, task.Id);
        await _unitOfWork.Notifications.AddAsync(notification, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return milestone.Id;
    }
}
