using MediatR;
using TaskManagerPro.Blazor.Domain.Entities;
using TaskManagerPro.Blazor.Domain.Interfaces;

namespace TaskManagerPro.Blazor.Application.Features.Tasks.Commands.CreateTask;

public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateTaskCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        var task = new TaskItem(request.Title, request.Description, request.DueDate, request.Priority, request.UserId);
        await _unitOfWork.Tasks.AddAsync(task, cancellationToken);

        var notification = new Notification("Task created", $"Task '{request.Title}' has been created.", request.UserId, task.Id);
        await _unitOfWork.Notifications.AddAsync(notification, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return task.Id;
    }
}
