using MediatR;
using TaskManagerPro.Blazor.Domain.Interfaces;

namespace TaskManagerPro.Blazor.Application.Features.Users.Queries.GetAllUsers;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, IEnumerable<UserDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllUsersQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _unitOfWork.Users.GetAllAsync(cancellationToken);

        return users
            .Where(u => !u.IsDeleted)
            .Select(u => new UserDto(u.Id, $"{u.FirstName} {u.LastName}".Trim(), u.Email))
            .OrderBy(u => u.FullName)
            .ToList();
    }
}
