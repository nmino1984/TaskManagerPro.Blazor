using MediatR;
using Microsoft.Extensions.Logging;
using TaskManagerPro.Blazor.Application.Common.Exceptions;
using TaskManagerPro.Blazor.Domain.Entities;
using TaskManagerPro.Blazor.Domain.Interfaces;

namespace TaskManagerPro.Blazor.Application.Features.SubTasks.Commands.UpdateSubTask;

public class UpdateSubTaskCommandHandler : IRequestHandler<UpdateSubTaskCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateSubTaskCommandHandler> _logger;

    public UpdateSubTaskCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateSubTaskCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(UpdateSubTaskCommand request, CancellationToken cancellationToken)
    {
        var subtask = await _unitOfWork.SubTasks.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(SubTask), request.Id);

        bool wasCompleted = subtask.IsCompleted;
        subtask.Update(request.Title, request.Description, request.IsCompleted);

        await _unitOfWork.SubTasks.UpdateAsync(subtask, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (!wasCompleted && subtask.IsCompleted)
            _logger.LogInformation("SubTask {SubTaskId} completed in Task {TaskId}", subtask.Id, subtask.TaskItemId);
    }
}
