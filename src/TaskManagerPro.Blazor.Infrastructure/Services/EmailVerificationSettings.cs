using Microsoft.Extensions.Configuration;
using TaskManagerPro.Blazor.Application.Common.Interfaces;

namespace TaskManagerPro.Blazor.Infrastructure.Services;

public class EmailVerificationSettings : IEmailVerificationSettings
{
    public int ExpirationMinutes { get; }
    public string BaseUrl { get; }

    public EmailVerificationSettings(IConfiguration configuration)
    {
        ExpirationMinutes = int.Parse(configuration["EmailVerification:ExpirationMinutes"] ?? "60");
        BaseUrl = configuration["EmailVerification:BaseUrl"] ?? "http://localhost:5026";
    }
}
