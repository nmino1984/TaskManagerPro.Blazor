using MediatR;

namespace TaskManagerPro.Blazor.Application.Features.Users.Queries.GetAllUsers;

public record GetAllUsersQuery : IRequest<IEnumerable<UserDto>>;
