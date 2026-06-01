using MediatR;

namespace TaskManagerPro.Blazor.Application.Features.Notifications.Commands.MarkAsRead;

/// <summary>
/// Command to mark a notification as read. UserId is required to ensure
/// a user can only read their own notifications.
/// </summary>
public record MarkAsReadCommand(Guid NotificationId, Guid UserId) : IRequest;
