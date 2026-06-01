using MediatR;

namespace TaskManagerPro.Blazor.Application.Features.Auth.Commands.Register;

/// <summary>
/// Command to register a new user account. Returns the new user's Domain Id
/// so the caller can correlate the Identity record with the Domain AppUser.
/// </summary>
public record RegisterCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password
) : IRequest<Guid>;
