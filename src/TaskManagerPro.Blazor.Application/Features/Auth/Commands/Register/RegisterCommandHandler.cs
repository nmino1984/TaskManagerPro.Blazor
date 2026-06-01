using MediatR;
using TaskManagerPro.Blazor.Application.Common.Interfaces;
using TaskManagerPro.Blazor.Domain.Entities;
using TaskManagerPro.Blazor.Domain.Interfaces;
using AppValidationException = TaskManagerPro.Blazor.Application.Common.Exceptions.ValidationException;

namespace TaskManagerPro.Blazor.Application.Features.Auth.Commands.Register;

/// <summary>
/// Creates a Domain AppUser record. Password is hashed via IPasswordHasher
/// before being stored so the handler never touches plain-text credentials.
/// Duplicate email check prevents silent overwrites of existing accounts.
/// </summary>
public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterCommandHandler(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task<Guid> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        IEnumerable<AppUser> existing = await _unitOfWork.Users.FindAsync(
            u => u.Email == request.Email, cancellationToken);

        if (existing.Any())
            throw new AppValidationException(new Dictionary<string, string[]>
            {
                { "Email", new[] { "This email address is already registered." } }
            });

        string passwordHash = _passwordHasher.Hash(request.Password);
        AppUser user = new(request.FirstName, request.LastName, request.Email, passwordHash);

        await _unitOfWork.Users.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}
