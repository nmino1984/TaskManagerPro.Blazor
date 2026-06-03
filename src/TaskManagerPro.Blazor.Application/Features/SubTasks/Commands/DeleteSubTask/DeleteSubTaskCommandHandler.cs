using MediatR;
using TaskManagerPro.Blazor.Application.Common.Exceptions;
using TaskManagerPro.Blazor.Domain.Entities;
using TaskManagerPro.Blazor.Domain.Interfaces;

namespace TaskManagerPro.Blazor.Application.Features.SubTasks.Commands.DeleteSubTask;

public class DeleteSubTaskCommandHandler : IRequestHandler<DeleteSubTaskCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteSubTaskCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteSubTaskCommand request, CancellationToken cancellationToken)
    {
        var subtask = await _unitOfWork.SubTasks.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(SubTask), request.Id);

        await _unitOfWork.SubTasks.DeleteAsync(subtask, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
