using MediatR;

namespace TaskManagerPro.Blazor.Application.Features.Milestones.Commands.CreateMilestone;

public record CreateMilestoneCommand(
    string Title,
    string Description,
    DateTime TargetDate,
    Guid TaskItemId
) : IRequest<Guid>;
