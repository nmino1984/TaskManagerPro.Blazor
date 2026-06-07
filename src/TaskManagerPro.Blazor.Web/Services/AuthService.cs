using System.IdentityModel.Tokens.Jwt;
using MediatR;
using Microsoft.AspNetCore.Components;
using TaskManagerPro.Blazor.Application.Features.Auth.Commands.Login;
using TaskManagerPro.Blazor.Application.Features.Auth.Commands.RefreshToken;
using TaskManagerPro.Blazor.Application.Features.Auth.Commands.Register;
using AppValidationException = TaskManagerPro.Blazor.Application.Common.Exceptions.ValidationException;

namespace TaskManagerPro.Blazor.Web.Services;

/// <summary>
/// Facade that wires Blazor UI actions to Application layer commands via MediatR,
/// then delegates auth-state transitions to CustomAuthStateProvider.
/// Keeps pages free of MediatR and auth plumbing.
/// </summary>
public class AuthService
{
    private readonly IMediator _mediator;
    private readonly CustomAuthStateProvider _authStateProvider;
    private readonly NavigationManager _navigation;
    private readonly AvatarStateService _avatarState;

    public AuthService(IMediator mediator, CustomAuthStateProvider authStateProvider, NavigationManager navigation, AvatarStateService avatarState)
    {
        _mediator = mediator;
        _authStateProvider = authStateProvider;
        _navigation = navigation;
        _avatarState = avatarState;
    }

    public async Task<LoginResult> LoginAsync(string email, string password)
    {
        try
        {
            var result = await _mediator.Send(new LoginCommand(email, password));
            await _authStateProvider.MarkUserAsAuthenticated(result.Token);
            return result;
        }
        catch (AppValidationException ex)
        {
            // ex.Message is the generic "One or more validation failures" — extract the
            // first concrete error (e.g. "Invalid email or password.") for the UI instead
            var message = ex.Errors.Values.SelectMany(e => e).FirstOrDefault() ?? ex.Message;
            throw new InvalidOperationException(message);
        }
    }

    public async Task<Guid> RegisterAsync(string firstName, string lastName, string email, string password)
    {
        try
        {
            return await _mediator.Send(new RegisterCommand(firstName, lastName, email, password));
            // Registration does not auto-login — caller should redirect to /login on success
        }
        catch (AppValidationException ex)
        {
            var message = ex.Errors.Values.SelectMany(e => e).FirstOrDefault() ?? ex.Message;
            throw new InvalidOperationException(message);
        }
    }

    public async Task RefreshTokenAsync(Guid userId)
    {
        var token = await _mediator.Send(new RefreshTokenCommand(userId));
        await _authStateProvider.MarkUserAsAuthenticated(token);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        var avatarUrl = jwt.Claims.FirstOrDefault(c => c.Type == "avatar_url")?.Value;
        _avatarState.SetAvatar(string.IsNullOrEmpty(avatarUrl) ? null : avatarUrl);
    }

    public async Task LogoutAsync()
    {
        await _authStateProvider.MarkUserAsLoggedOut();
        _navigation.NavigateTo("/login");
    }
}
