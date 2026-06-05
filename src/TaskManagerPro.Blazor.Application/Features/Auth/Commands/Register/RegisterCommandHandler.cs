using MediatR;
using TaskManagerPro.Blazor.Application.Common.Interfaces;
using TaskManagerPro.Blazor.Domain.Entities;
using TaskManagerPro.Blazor.Domain.Interfaces;
using AppValidationException = TaskManagerPro.Blazor.Application.Common.Exceptions.ValidationException;

namespace TaskManagerPro.Blazor.Application.Features.Auth.Commands.Register;

/// <summary>
/// Password is hashed via IPasswordHasher before storage — plain text never touches this handler.
/// Duplicate email check prevents silent overwrites of existing accounts.
/// Both AppUser (domain) and ApplicationUser (Identity) are created so the user can log in.
/// </summary>
public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUserRegistrationService _registrationService;

    public RegisterCommandHandler(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher, IUserRegistrationService registrationService)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _registrationService = registrationService;
    }

    public async Task<Guid> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var existing = await _unitOfWork.Users.FindAsync(u => u.Email == request.Email, cancellationToken);

        if (existing.Any())
            throw new AppValidationException(new Dictionary<string, string[]>
            {
                { "Email", new[] { "This email address is already registered." } }
            });

        var user = new AppUser(request.FirstName, request.LastName, request.Email,
                               _passwordHasher.Hash(request.Password));

        await _unitOfWork.Users.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Create the Identity record with the same Id so both tables stay in sync
        await _registrationService.CreateIdentityUserAsync(
            user.Id, request.FirstName, request.LastName, request.Email, request.Password, cancellationToken);

        return user.Id;
    }
}
