using MediatR;

namespace TaskManagerPro.Blazor.Application.Features.Auth.Commands.UpdateProfile;

public record UpdateProfileCommand(Guid UserId, string FirstName, string LastName, string Email) : IRequest;
