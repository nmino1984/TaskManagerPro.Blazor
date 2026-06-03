using MediatR;
using TaskManagerPro.Blazor.Application.Common.Exceptions;
using TaskManagerPro.Blazor.Domain.Entities;
using TaskManagerPro.Blazor.Domain.Interfaces;

namespace TaskManagerPro.Blazor.Application.Features.SubTasks.Commands.UpdateSubTask;

public class UpdateSubTaskCommandHandler : IRequestHandler<UpdateSubTaskCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateSubTaskCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateSubTaskCommand request, CancellationToken cancellationToken)
    {
        var subtask = await _unitOfWork.SubTasks.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(SubTask), request.Id);

        subtask.Update(request.Title, request.Description, request.IsCompleted);

        await _unitOfWork.SubTasks.UpdateAsync(subtask, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
