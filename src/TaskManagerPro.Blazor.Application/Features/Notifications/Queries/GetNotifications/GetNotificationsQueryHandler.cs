using MediatR;
using TaskManagerPro.Blazor.Domain.Interfaces;

namespace TaskManagerPro.Blazor.Application.Features.Notifications.Queries.GetNotifications;

/// <summary>
/// Fetches all notifications for a user using FindAsync so the UserId filter
/// is evaluated in the database rather than in memory.
/// </summary>
public class GetNotificationsQueryHandler : IRequestHandler<GetNotificationsQuery, IEnumerable<NotificationDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetNotificationsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<NotificationDto>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Domain.Entities.Notification> notifications = await _unitOfWork.Notifications.FindAsync(
            n => n.UserId == request.UserId, cancellationToken);

        return notifications.Select(n => new NotificationDto(
            n.Id,
            n.Title,
            n.Message,
            n.Status,
            n.UserId,
            n.TaskItemId,
            n.CreatedAt,
            n.UpdatedAt
        ));
    }
}
