namespace TaskManagerPro.Blazor.Application.Common.Interfaces;

public interface IEmailService
{
    Task SendVerificationEmailAsync(string toEmail, string toName, string verificationLink, CancellationToken ct);
}
