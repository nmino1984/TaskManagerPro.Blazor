using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;
using TaskManagerPro.Blazor.Application.Common.Interfaces;

namespace TaskManagerPro.Blazor.Infrastructure.Services;

/// <summary>
/// Uses SendGrid rather than SMTP because SendGrid handles deliverability,
/// bounce management, and unsubscribe compliance out of the box — critical
/// for transactional email in production. API key is read from configuration
/// so it never lives in source control.
/// </summary>
public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendVerificationEmailAsync(string toEmail, string toName, string verificationLink, CancellationToken ct)
    {
        var apiKey = _configuration["SendGrid:ApiKey"];
        var fromEmail = _configuration["SendGrid:FromEmail"] ?? string.Empty;
        var fromName = _configuration["SendGrid:FromName"] ?? "TaskManager Pro";

        var client = new SendGridClient(apiKey);
        var from = new EmailAddress(fromEmail, fromName);
        var to = new EmailAddress(toEmail, toName);

        var subject = "Verify your TaskManager Pro account";
        var htmlContent = $"""
            <div style="font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;">
                <h2 style="color: #7B2FBE;">Hey {toName}, welcome aboard!</h2>
                <p>Thanks for signing up. One quick step before you get started — just confirm your email address and you're all set.</p>
                <p style="margin: 32px 0;">
                    <a href="{verificationLink}"
                       style="background-color: #7B2FBE; color: white; padding: 12px 28px;
                              text-decoration: none; border-radius: 6px; font-weight: bold;">
                        Confirm my email
                    </a>
                </p>
                <p style="color: #666; font-size: 14px;">
                    This link is only valid for {_configuration["EmailVerification:ExpirationMinutes"] ?? "60"} minutes, so don't wait too long.<br/>
                    Didn't sign up for TaskManager Pro? No worries — just ignore this email.
                </p>
            </div>
            """;

        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent: null, htmlContent);
        var response = await client.SendEmailAsync(msg, ct);

        if ((int)response.StatusCode >= 400)
        {
            var body = await response.Body.ReadAsStringAsync(ct);
            _logger.LogError("SendGrid returned {StatusCode} for {Email}. Body: {Body}",
                (int)response.StatusCode, toEmail, body);
        }
    }
}
