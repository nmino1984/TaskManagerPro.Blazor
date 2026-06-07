using MediatR;
using Microsoft.Extensions.Logging;
using TaskManagerPro.Blazor.Domain.Interfaces;

namespace TaskManagerPro.Blazor.Application.Features.Auth.Commands.VerifyEmail;

public class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommand, VerifyEmailResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<VerifyEmailCommandHandler> _logger;

    public VerifyEmailCommandHandler(IUnitOfWork unitOfWork, ILogger<VerifyEmailCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<VerifyEmailResult> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        var users = await _unitOfWork.Users.FindAsync(u => u.VerificationToken == request.Token, cancellationToken);
        var user = users.FirstOrDefault();

        if (user is null)
            return new VerifyEmailResult(false, "Invalid or expired token.");

        if (user.IsEmailVerified)
            return new VerifyEmailResult(true, "Email already verified.");

        if (user.VerificationTokenExpiry < DateTime.UtcNow)
            return new VerifyEmailResult(false, "Token has expired. Please request a new verification email.");

        user.VerifyEmail();

        await _unitOfWork.Users.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Email verified for user {UserId}", user.Id);

        return new VerifyEmailResult(true, "Email verified successfully.");
    }
}
