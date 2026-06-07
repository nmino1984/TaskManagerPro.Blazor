using MediatR;
using Microsoft.Extensions.Logging;
using TaskManagerPro.Blazor.Application.Common.Interfaces;
using TaskManagerPro.Blazor.Domain.Entities;
using TaskManagerPro.Blazor.Domain.Interfaces;
using AppValidationException = TaskManagerPro.Blazor.Application.Common.Exceptions.ValidationException;

namespace TaskManagerPro.Blazor.Application.Features.Auth.Commands.Register;

/// <summary>
/// Creates both AppUser (domain) and ApplicationUser (Identity) so the two stores stay in sync.
/// Login is allowed before email verification — IsEmailVerified on LoginResult signals the UI.
/// </summary>
public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUserRegistrationService _registrationService;
    private readonly IEmailService _emailService;
    private readonly IEmailVerificationSettings _verificationSettings;
    private readonly ILogger<RegisterCommandHandler> _logger;

    public RegisterCommandHandler(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IUserRegistrationService registrationService,
        IEmailService emailService,
        IEmailVerificationSettings verificationSettings,
        ILogger<RegisterCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _registrationService = registrationService;
        _emailService = emailService;
        _verificationSettings = verificationSettings;
        _logger = logger;
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

        var token = Guid.NewGuid().ToString("N");
        user.SetVerificationToken(token, DateTime.UtcNow.AddMinutes(_verificationSettings.ExpirationMinutes));

        await _unitOfWork.Users.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Create the Identity record with the same Id so both tables stay in sync
        await _registrationService.CreateIdentityUserAsync(
            user.Id, request.FirstName, request.LastName, request.Email, request.Password, cancellationToken);

        _logger.LogInformation("User registered: {Email} (Id: {UserId})", request.Email, user.Id);

        var verificationLink = $"{_verificationSettings.BaseUrl}/verify-email?token={token}";
        try
        {
            await _emailService.SendVerificationEmailAsync(request.Email, request.FirstName, verificationLink, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send verification email to {Email}", request.Email);
        }

        return user.Id;
    }
}
