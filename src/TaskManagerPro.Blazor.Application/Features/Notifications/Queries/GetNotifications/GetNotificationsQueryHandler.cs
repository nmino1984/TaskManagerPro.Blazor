using MediatR;
using TaskManagerPro.Blazor.Domain.Interfaces;

namespace TaskManagerPro.Blazor.Application.Features.Notifications.Queries.GetNotifications;

public class GetNotificationsQueryHandler : IRequestHandler<GetNotificationsQuery, IEnumerable<NotificationDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetNotificationsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<NotificationDto>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
    {
        var notifications = await _unitOfWork.Notifications.FindAsync(
            n => n.UserId == request.UserId, cancellationToken);

        return notifications.Select(n => new NotificationDto(
            n.Id, n.Title, n.Message, n.Status, n.UserId, n.TaskItemId, n.CreatedAt, n.UpdatedAt));
    }
}
