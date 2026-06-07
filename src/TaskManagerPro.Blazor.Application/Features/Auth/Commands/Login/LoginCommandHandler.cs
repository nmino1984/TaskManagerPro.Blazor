using MediatR;
using Microsoft.Extensions.Logging;
using TaskManagerPro.Blazor.Application.Common.Interfaces;
using TaskManagerPro.Blazor.Domain.Interfaces;
using AppValidationException = TaskManagerPro.Blazor.Application.Common.Exceptions.ValidationException;

namespace TaskManagerPro.Blazor.Application.Features.Auth.Commands.Login;

/// <summary>
/// "User not found" and "wrong password" return the same error message
/// to prevent user enumeration via different responses.
/// </summary>
public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher, IJwtTokenGenerator jwtTokenGenerator, ILogger<LoginCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _logger = logger;
    }

    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var users = await _unitOfWork.Users.FindAsync(u => u.Email == request.Email, cancellationToken);
        var user = users.FirstOrDefault();

        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new AppValidationException(new Dictionary<string, string[]>
            {
                { "Credentials", new[] { "Invalid email or password." } }
            });

        // Expired token — block login and force them to request a new verification email
        if (!user.IsEmailVerified && user.VerificationTokenExpiry < DateTime.UtcNow)
        {
            _logger.LogWarning("Login blocked for {Email} — email not verified and token expired", request.Email);
            throw new AppValidationException(new Dictionary<string, string[]>
            {
                { "Email", new[] { "Your account is not verified and the verification link has expired. Please request a new one." } }
            });
        }

        var (token, expiresAt) = _jwtTokenGenerator.GenerateToken(user.Id, user.Email, user.FirstName, user.LastName, user.IsEmailVerified);

        // Token still valid — allow login but signal the UI to show a verification reminder
        return new LoginResult(token, user.Id, user.Email, expiresAt, user.IsEmailVerified);
    }
}
