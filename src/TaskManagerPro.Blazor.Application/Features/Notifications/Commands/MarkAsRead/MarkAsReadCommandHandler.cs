using MediatR;
using TaskManagerPro.Blazor.Application.Common.Exceptions;
using TaskManagerPro.Blazor.Domain.Entities;
using TaskManagerPro.Blazor.Domain.Interfaces;
using AppValidationException = TaskManagerPro.Blazor.Application.Common.Exceptions.ValidationException;

namespace TaskManagerPro.Blazor.Application.Features.Notifications.Commands.MarkAsRead;

/// <summary>
/// Transitions a notification to Read state after verifying ownership.
/// Using the domain MarkAsRead() method keeps the status transition inside the aggregate.
/// </summary>
public class MarkAsReadCommandHandler : IRequestHandler<MarkAsReadCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public MarkAsReadCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(MarkAsReadCommand request, CancellationToken cancellationToken)
    {
        Notification? notification = await _unitOfWork.Notifications.GetByIdAsync(request.NotificationId, cancellationToken);

        if (notification is null)
            throw new NotFoundException(nameof(Notification), request.NotificationId);

        if (notification.UserId != request.UserId)
            throw new AppValidationException(new Dictionary<string, string[]>
            {
                { "Authorization", new[] { "You are not authorized to read this notification." } }
            });

        notification.MarkAsRead();

        await _unitOfWork.Notifications.UpdateAsync(notification, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
