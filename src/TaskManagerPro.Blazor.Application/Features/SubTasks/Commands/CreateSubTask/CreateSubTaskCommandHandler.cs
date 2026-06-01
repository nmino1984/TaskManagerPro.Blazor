using MediatR;
using TaskManagerPro.Blazor.Application.Common.Exceptions;
using TaskManagerPro.Blazor.Domain.Entities;
using TaskManagerPro.Blazor.Domain.Interfaces;

namespace TaskManagerPro.Blazor.Application.Features.SubTasks.Commands.CreateSubTask;

/// <summary>
/// Creates a subtask after verifying the parent task exists.
/// The existence check prevents orphaned subtasks with invalid TaskItemIds.
/// </summary>
public class CreateSubTaskCommandHandler : IRequestHandler<CreateSubTaskCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateSubTaskCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateSubTaskCommand request, CancellationToken cancellationToken)
    {
        TaskItem? parent = await _unitOfWork.Tasks.GetByIdAsync(request.TaskItemId, cancellationToken);

        if (parent is null)
            throw new NotFoundException(nameof(TaskItem), request.TaskItemId);

        SubTask subTask = new(request.Title, request.Description, request.TaskItemId);

        await _unitOfWork.SubTasks.AddAsync(subTask, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return subTask.Id;
    }
}
