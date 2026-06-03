using MediatR;
using TaskManagerPro.Blazor.Application.Common.Exceptions;
using TaskManagerPro.Blazor.Domain.Entities;
using TaskManagerPro.Blazor.Domain.Interfaces;
using AppValidationException = TaskManagerPro.Blazor.Application.Common.Exceptions.ValidationException;

namespace TaskManagerPro.Blazor.Application.Features.Tasks.Commands.DeleteTask;

// Ownership check prevents a user from deleting another user's tasks
public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteTaskCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _unitOfWork.Tasks.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(TaskItem), request.Id);

        if (task.UserId != request.UserId)
            throw new AppValidationException(new Dictionary<string, string[]>
            {
                { "Authorization", new[] { "You are not authorized to delete this task." } }
            });

        await _unitOfWork.Tasks.DeleteAsync(task, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
