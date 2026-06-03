using MediatR;

namespace TaskManagerPro.Blazor.Application.Features.SubTasks.Commands.UpdateSubTask;

public record UpdateSubTaskCommand(
    Guid Id,
    string Title,
    string Description,
    bool IsCompleted
) : IRequest;
