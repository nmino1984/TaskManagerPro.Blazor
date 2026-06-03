using MediatR;

namespace TaskManagerPro.Blazor.Application.Features.Notifications.Commands.MarkAsRead;

// UserId required so a user can only mark their own notifications as read
public record MarkAsReadCommand(Guid NotificationId, Guid UserId) : IRequest;
