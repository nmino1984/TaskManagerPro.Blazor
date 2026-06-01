using MediatR;

namespace TaskManagerPro.Blazor.Application.Features.Milestones.Queries.GetMilestonesByTask;

/// <summary>
/// Query to retrieve all non-deleted milestones belonging to a specific task.
/// </summary>
public record GetMilestonesByTaskQuery(Guid TaskItemId) : IRequest<IEnumerable<MilestoneDto>>;
