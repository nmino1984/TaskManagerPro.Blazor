using TaskManagerPro.Blazor.Domain.Enums;

namespace TaskManagerPro.Blazor.Application.Features.Notifications.Queries.GetNotifications;

/// <summary>
/// Read model for a notification. Includes Status so the UI can distinguish
/// unread alerts without making a separate request.
/// </summary>
public record NotificationDto(
    Guid Id,
    string Title,
    string Message,
    NotificationStatus Status,
    Guid UserId,
    Guid? TaskItemId,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
