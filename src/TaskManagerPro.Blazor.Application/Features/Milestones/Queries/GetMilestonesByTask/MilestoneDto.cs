using TaskManagerPro.Blazor.Domain.Enums;

namespace TaskManagerPro.Blazor.Application.Features.Milestones.Queries.GetMilestonesByTask;

/// <summary>
/// Read model for a milestone. Includes Status and TargetDate so the UI
/// can render deadline-aware progress indicators.
/// </summary>
public record MilestoneDto(
    Guid Id,
    string Title,
    string Description,
    DateTime TargetDate,
    MilestoneStatus Status,
    Guid TaskItemId,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
