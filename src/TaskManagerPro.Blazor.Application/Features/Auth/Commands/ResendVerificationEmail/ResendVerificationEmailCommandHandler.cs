using MediatR;
using Microsoft.Extensions.Logging;
using TaskManagerPro.Blazor.Application.Common.Interfaces;
using TaskManagerPro.Blazor.Domain.Interfaces;

namespace TaskManagerPro.Blazor.Application.Features.Auth.Commands.ResendVerificationEmail;

public class ResendVerificationEmailCommandHandler : IRequestHandler<ResendVerificationEmailCommand, ResendVerificationEmailResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly IEmailVerificationSettings _verificationSettings;
    private readonly ILogger<ResendVerificationEmailCommandHandler> _logger;

    public ResendVerificationEmailCommandHandler(
        IUnitOfWork unitOfWork,
        IEmailService emailService,
        IEmailVerificationSettings verificationSettings,
        ILogger<ResendVerificationEmailCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _emailService = emailService;
        _verificationSettings = verificationSettings;
        _logger = logger;
    }

    public async Task<ResendVerificationEmailResult> Handle(ResendVerificationEmailCommand request, CancellationToken cancellationToken)
    {
        var users = await _unitOfWork.Users.FindAsync(u => u.Email == request.Email, cancellationToken);
        var user = users.FirstOrDefault();

        // Intentionally vague when user not found — prevents email enumeration
        if (user is null)
        {
            _logger.LogWarning("Resend verification requested for unknown email {Email}", request.Email);
            return new ResendVerificationEmailResult(false, "If that email exists, a new verification link has been sent.");
        }

        if (user.IsEmailVerified)
            return new ResendVerificationEmailResult(false, "Your email is already verified.");

        var token = Guid.NewGuid().ToString("N");
        user.SetVerificationToken(token, DateTime.UtcNow.AddMinutes(_verificationSettings.ExpirationMinutes));

        await _unitOfWork.Users.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var verificationLink = $"{_verificationSettings.BaseUrl}/verify-email?token={token}";
        await _emailService.SendVerificationEmailAsync(user.Email, user.FirstName, verificationLink, cancellationToken);

        _logger.LogInformation("Verification email resent to {Email} (UserId: {UserId})", user.Email, user.Id);

        return new ResendVerificationEmailResult(true, "A new verification email has been sent. Please check your inbox.");
    }
}
