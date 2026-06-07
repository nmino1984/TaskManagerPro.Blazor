using MediatR;
using TaskManagerPro.Blazor.Application.Common.Exceptions;
using TaskManagerPro.Blazor.Domain.Entities;
using TaskManagerPro.Blazor.Domain.Interfaces;

namespace TaskManagerPro.Blazor.Application.Features.SubTasks.Commands.CreateSubTask;

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

    // TODO: consider moving validation here instead of relying only on the validator
    private static bool HasValidParent(Guid taskItemId) => taskItemId != Guid.Empty;
}
