using MediatR;

namespace TaskManagerPro.Blazor.Application.Features.SubTasks.Queries.GetSubTasksByTask;

public record GetSubTasksByTaskQuery(Guid TaskItemId) : IRequest<IEnumerable<SubTaskDto>>;
