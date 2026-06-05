using MediatR;
using TaskManagerPro.Blazor.Application.Common.Exceptions;
using TaskManagerPro.Blazor.Application.Common.Interfaces;
using TaskManagerPro.Blazor.Domain.Entities;
using TaskManagerPro.Blazor.Domain.Interfaces;
using AppValidationException = TaskManagerPro.Blazor.Application.Common.Exceptions.ValidationException;

namespace TaskManagerPro.Blazor.Application.Features.Auth.Commands.ChangePassword;

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IIdentityUserService _identityService;

    public ChangePasswordCommandHandler(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher, IIdentityUserService identityService)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _identityService = identityService;
    }

    public async Task Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken)
            ?? throw new NotFoundException(nameof(AppUser), request.UserId);

        if (!_passwordHasher.Verify(request.CurrentPassword, user.PasswordHash))
            throw new AppValidationException(new Dictionary<string, string[]>
            {
                { "CurrentPassword", new[] { "Current password is incorrect." } }
            });

        user.PasswordHash = _passwordHasher.Hash(request.NewPassword);

        await _unitOfWork.Users.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _identityService.ResetPasswordAsync(request.UserId, request.NewPassword, cancellationToken);
    }
}
