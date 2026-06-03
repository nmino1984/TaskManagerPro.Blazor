using TaskManagerPro.Blazor.Domain.Enums;

namespace TaskManagerPro.Blazor.Application.Features.Milestones.Queries.GetMilestonesByTask;

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
