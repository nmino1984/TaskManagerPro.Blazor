namespace TaskManagerPro.Blazor.Application.Features.Auth.Queries.GetUserProfile;

public record UserProfileDto(Guid Id, string FirstName, string LastName, string Email);
