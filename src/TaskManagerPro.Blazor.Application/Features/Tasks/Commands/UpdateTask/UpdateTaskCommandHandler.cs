using MediatR;
using TaskManagerPro.Blazor.Application.Common.Exceptions;
using TaskManagerPro.Blazor.Domain.Entities;
using TaskManagerPro.Blazor.Domain.Enums;
using TaskManagerPro.Blazor.Domain.Interfaces;

namespace TaskManagerPro.Blazor.Application.Features.Tasks.Commands.UpdateTask;

/// <summary>
/// Applies field updates to an existing task and, when a Status is supplied,
/// drives the lifecycle transition through the aggregate's own domain methods
/// so business rules are never bypassed.
/// </summary>
public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Initialises the handler with the unit-of-work dependency.</summary>
    public UpdateTaskCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <summary>Handles the UpdateTaskCommand request.</summary>
    public async Task Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        TaskItem? task = await _unitOfWork.Tasks.GetByIdAsync(request.Id, cancellationToken);

        if (task is null)
            throw new NotFoundException(nameof(TaskItem), request.Id);

        task.Update(request.Title, request.Description, request.DueDate, request.Priority);

        if (request.Status.HasValue)
        {
            switch (request.Status.Value)
            {
                case WorkTaskStatus.InProgress:
                    task.StartProgress();
                    break;
                case WorkTaskStatus.Completed:
                    task.Complete();
                    break;
                case WorkTaskStatus.Cancelled:
                    task.Cancel();
                    break;
                case WorkTaskStatus.Pending:
                    task.ResetToPending();
                    break;
            }
        }

        await _unitOfWork.Tasks.UpdateAsync(task, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
