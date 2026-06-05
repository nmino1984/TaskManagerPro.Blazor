using MediatR;
using TaskManagerPro.Blazor.Application.Common.Exceptions;
using TaskManagerPro.Blazor.Application.Common.Interfaces;
using TaskManagerPro.Blazor.Domain.Entities;
using TaskManagerPro.Blazor.Domain.Interfaces;
using AppValidationException = TaskManagerPro.Blazor.Application.Common.Exceptions.ValidationException;

namespace TaskManagerPro.Blazor.Application.Features.Auth.Commands.UpdateProfile;

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdentityUserService _identityService;

    public UpdateProfileCommandHandler(IUnitOfWork unitOfWork, IIdentityUserService identityService)
    {
        _unitOfWork = unitOfWork;
        _identityService = identityService;
    }

    public async Task Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken)
            ?? throw new NotFoundException(nameof(AppUser), request.UserId);

        if (!string.Equals(user.Email, request.Email, StringComparison.OrdinalIgnoreCase))
        {
            var existing = await _unitOfWork.Users.FindAsync(
                u => u.Email == request.Email && u.Id != request.UserId, cancellationToken);

            if (existing.Any())
                throw new AppValidationException(new Dictionary<string, string[]>
                {
                    { "Email", new[] { "This email address is already in use." } }
                });
        }

        user.FirstName = request.FirstName;
        user.LastName  = request.LastName;
        user.Email     = request.Email;

        await _unitOfWork.Users.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _identityService.UpdateUserAsync(
            request.UserId, request.Email, request.FirstName, request.LastName, cancellationToken);
    }
}
