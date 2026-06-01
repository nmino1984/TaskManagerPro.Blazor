using MediatR;

namespace TaskManagerPro.Blazor.Application.Features.Notifications.Queries.GetNotifications;

/// <summary>
/// Query to retrieve all non-deleted notifications for a specific user.
/// </summary>
public record GetNotificationsQuery(Guid UserId) : IRequest<IEnumerable<NotificationDto>>;
