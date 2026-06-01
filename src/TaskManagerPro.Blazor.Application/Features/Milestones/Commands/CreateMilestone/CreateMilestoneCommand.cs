using MediatR;

namespace TaskManagerPro.Blazor.Application.Features.Milestones.Commands.CreateMilestone;

/// <summary>
/// Command to add a new milestone checkpoint to an existing task.
/// </summary>
public record CreateMilestoneCommand(
    string Title,
    string Description,
    DateTime TargetDate,
    Guid TaskItemId
) : IRequest<Guid>;
