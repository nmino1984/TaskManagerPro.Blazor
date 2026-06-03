using MediatR;

namespace TaskManagerPro.Blazor.Application.Features.Milestones.Commands.DeleteMilestone;

public record DeleteMilestoneCommand(Guid Id) : IRequest;
