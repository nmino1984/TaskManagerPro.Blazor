using TaskManagerPro.Blazor.Domain.Enums;

namespace TaskManagerPro.Blazor.Application.Features.Notifications.Queries.GetNotifications;

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
