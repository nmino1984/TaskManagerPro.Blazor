using MediatR;
using TaskManagerPro.Blazor.Application.Common.Exceptions;
using TaskManagerPro.Blazor.Domain.Entities;
using TaskManagerPro.Blazor.Domain.Enums;
using TaskManagerPro.Blazor.Domain.Interfaces;

namespace TaskManagerPro.Blazor.Application.Features.Tasks.Commands.UpdateTask;

/// <summary>
/// Status transitions go through domain methods so business rules are never bypassed.
/// </summary>
public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTaskCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _unitOfWork.Tasks.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(TaskItem), request.Id);

        WorkTaskStatus previousStatus = task.Status;
        Guid? previousAssignedTo = task.AssignedToUserId;
        task.Update(request.Title, request.Description, request.DueDate, request.Priority, request.AssignedToUserId);

        if (request.Status.HasValue)
        {
            switch (request.Status.Value)
            {
                case WorkTaskStatus.InProgress: task.StartProgress(); break;
                case WorkTaskStatus.Completed:  task.Complete();      break;
                case WorkTaskStatus.Cancelled:  task.Cancel();        break;
                case WorkTaskStatus.Pending:    task.ResetToPending(); break;
            }
        }

        await _unitOfWork.Tasks.UpdateAsync(task, cancellationToken);

        if (task.Status != previousStatus)
        {
            Notification? notification = task.Status switch
            {
                WorkTaskStatus.Completed => new Notification(
                    $"Task completed: '{task.Title}'",
                    $"Your task has been marked as completed.",
                    task.UserId, task.Id),
                WorkTaskStatus.Cancelled => new Notification(
                    $"Task cancelled: '{task.Title}'",
                    $"Your task has been cancelled.",
                    task.UserId, task.Id),
                _ => null
            };

            if (notification is not null)
                await _unitOfWork.Notifications.AddAsync(notification, cancellationToken);
        }

        if (request.AssignedToUserId.HasValue && request.AssignedToUserId != previousAssignedTo)
        {
            var assignmentNotification = new Notification(
                "Task assigned to you",
                $"'{task.Title}' has been assigned to you.",
                request.AssignedToUserId.Value, task.Id);
            await _unitOfWork.Notifications.AddAsync(assignmentNotification, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
