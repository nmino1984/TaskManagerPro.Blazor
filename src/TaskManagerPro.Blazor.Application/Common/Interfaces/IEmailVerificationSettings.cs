namespace TaskManagerPro.Blazor.Application.Common.Interfaces;

public interface IEmailVerificationSettings
{
    int ExpirationMinutes { get; }
    string BaseUrl { get; }
}
