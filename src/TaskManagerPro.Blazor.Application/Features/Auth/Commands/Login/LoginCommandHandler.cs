using MediatR;
using TaskManagerPro.Blazor.Application.Common.Exceptions;
using TaskManagerPro.Blazor.Application.Common.Interfaces;
using TaskManagerPro.Blazor.Domain.Entities;
using TaskManagerPro.Blazor.Domain.Interfaces;
using AppValidationException = TaskManagerPro.Blazor.Application.Common.Exceptions.ValidationException;

namespace TaskManagerPro.Blazor.Application.Features.Auth.Commands.Login;

/// <summary>
/// Validates credentials and issues a JWT. Both "user not found" and "wrong password"
/// return the same generic message to prevent user enumeration via error differences.
/// </summary>
public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginCommandHandler(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        IEnumerable<AppUser> users = await _unitOfWork.Users.FindAsync(
            u => u.Email == request.Email, cancellationToken);

        AppUser? user = users.FirstOrDefault();

        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new AppValidationException(new Dictionary<string, string[]>
            {
                { "Credentials", new[] { "Invalid email or password." } }
            });

        (string token, DateTime expiresAt) = _jwtTokenGenerator.GenerateToken(
            user.Id, user.Email, user.FirstName, user.LastName);

        return new LoginResult(token, user.Id, user.Email, expiresAt);
    }
}
