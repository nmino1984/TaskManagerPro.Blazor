using MediatR;

namespace TaskManagerPro.Blazor.Application.Features.Notifications.Queries.GetNotifications;

public record GetNotificationsQuery(Guid UserId) : IRequest<IEnumerable<NotificationDto>>;
