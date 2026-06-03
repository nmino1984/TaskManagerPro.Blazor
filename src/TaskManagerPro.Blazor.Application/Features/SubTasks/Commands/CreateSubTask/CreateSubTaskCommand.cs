using MediatR;

namespace TaskManagerPro.Blazor.Application.Features.SubTasks.Commands.CreateSubTask;

public record CreateSubTaskCommand(
    string Title,
    string Description,
    Guid TaskItemId
) : IRequest<Guid>;
