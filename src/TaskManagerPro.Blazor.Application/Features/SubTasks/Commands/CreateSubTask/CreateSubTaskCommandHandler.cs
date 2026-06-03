using MediatR;
using TaskManagerPro.Blazor.Application.Common.Exceptions;
using TaskManagerPro.Blazor.Domain.Entities;
using TaskManagerPro.Blazor.Domain.Interfaces;

namespace TaskManagerPro.Blazor.Application.Features.SubTasks.Commands.CreateSubTask;

// Verifies the parent task exists to prevent orphaned subtasks with an invalid TaskItemId
public class CreateSubTaskCommandHandler : IRequestHandler<CreateSubTaskCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateSubTaskCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateSubTaskCommand request, CancellationToken cancellationToken)
    {
        _ = await _unitOfWork.Tasks.GetByIdAsync(request.TaskItemId, cancellationToken)
            ?? throw new NotFoundException(nameof(TaskItem), request.TaskItemId);

        var subtask = new SubTask(request.Title, request.Description, request.TaskItemId);

        await _unitOfWork.SubTasks.AddAsync(subtask, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return subtask.Id;
    }
}
