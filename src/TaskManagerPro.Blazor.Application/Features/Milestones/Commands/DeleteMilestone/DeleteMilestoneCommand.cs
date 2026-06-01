using MediatR;

namespace TaskManagerPro.Blazor.Application.Features.Milestones.Commands.DeleteMilestone;

/// <summary>
/// Command to soft-delete a milestone by its identifier.
/// </summary>
public record DeleteMilestoneCommand(Guid Id) : IRequest;
