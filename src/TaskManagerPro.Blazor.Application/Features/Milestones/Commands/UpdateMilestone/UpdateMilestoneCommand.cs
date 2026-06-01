using MediatR;
using TaskManagerPro.Blazor.Domain.Enums;

namespace TaskManagerPro.Blazor.Application.Features.Milestones.Commands.UpdateMilestone;

/// <summary>
/// Command to update a milestone's fields and status.
/// Status is included here because milestone lifecycle (e.g. Overdue) is managed
/// by both user actions and background jobs via the same domain method.
/// </summary>
public record UpdateMilestoneCommand(
    Guid Id,
    string Title,
    string Description,
    DateTime TargetDate,
    MilestoneStatus Status
) : IRequest;
