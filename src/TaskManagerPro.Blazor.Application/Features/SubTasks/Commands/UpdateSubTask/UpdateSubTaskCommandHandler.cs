using MediatR;
using TaskManagerPro.Blazor.Application.Common.Exceptions;
using TaskManagerPro.Blazor.Domain.Entities;
using TaskManagerPro.Blazor.Domain.Interfaces;

namespace TaskManagerPro.Blazor.Application.Features.SubTasks.Commands.UpdateSubTask;

/// <summary>
/// Applies field updates to a subtask via the domain Update() method.
/// </summary>
public class UpdateSubTaskCommandHandler : IRequestHandler<UpdateSubTaskCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateSubTaskCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateSubTaskCommand request, CancellationToken cancellationToken)
    {
        SubTask? subTask = await _unitOfWork.SubTasks.GetByIdAsync(request.Id, cancellationToken);

        if (subTask is null)
            throw new NotFoundException(nameof(SubTask), request.Id);

        subTask.Update(request.Title, request.Description, request.IsCompleted);

        await _unitOfWork.SubTasks.UpdateAsync(subTask, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
