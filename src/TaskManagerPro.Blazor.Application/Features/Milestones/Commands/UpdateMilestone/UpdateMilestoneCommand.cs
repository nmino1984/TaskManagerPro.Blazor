using MediatR;
using TaskManagerPro.Blazor.Domain.Enums;

namespace TaskManagerPro.Blazor.Application.Features.Milestones.Commands.UpdateMilestone;

/// <summary>
/// Status is included so both user actions and background jobs (e.g. Overdue transitions)
/// can drive the lifecycle through the same domain method.
/// </summary>
public record UpdateMilestoneCommand(
    Guid Id,
    string Title,
    string Description,
    DateTime TargetDate,
    MilestoneStatus Status
) : IRequest;
