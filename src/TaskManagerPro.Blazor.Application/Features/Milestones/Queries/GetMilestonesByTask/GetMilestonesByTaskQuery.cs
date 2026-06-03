using MediatR;

namespace TaskManagerPro.Blazor.Application.Features.Milestones.Queries.GetMilestonesByTask;

public record GetMilestonesByTaskQuery(Guid TaskItemId) : IRequest<IEnumerable<MilestoneDto>>;
