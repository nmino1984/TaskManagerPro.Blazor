using MediatR;

namespace TaskManagerPro.Blazor.Application.Features.SubTasks.Commands.DeleteSubTask;

public record DeleteSubTaskCommand(Guid Id) : IRequest;
