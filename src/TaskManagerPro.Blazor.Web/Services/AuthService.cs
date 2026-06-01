using MediatR;
using TaskManagerPro.Blazor.Application.Features.Auth.Commands.Login;
using TaskManagerPro.Blazor.Application.Features.Auth.Commands.Register;
using AppValidationException = TaskManagerPro.Blazor.Application.Common.Exceptions.ValidationException;

namespace TaskManagerPro.Blazor.Web.Services;

/// <summary>
/// Facade that wires Blazor UI actions (login, register, logout) to Application
/// layer commands via MediatR, then delegates auth-state transitions to
/// CustomAuthStateProvider. Keeps pages free of MediatR and auth plumbing.
/// </summary>
public class AuthService
{
    private readonly IMediator _mediator;
    private readonly CustomAuthStateProvider _authStateProvider;

    /// <summary>
    /// Both dependencies are scoped to the Blazor circuit so state is never
    /// shared across users on a multi-user server.
    /// </summary>
    public AuthService(IMediator mediator, CustomAuthStateProvider authStateProvider)
    {
        _mediator = mediator;
        _authStateProvider = authStateProvider;
    }

    /// <summary>
    /// Sends LoginCommand to the Application layer, receives a signed JWT, and
    /// persists it via CustomAuthStateProvider so the circuit becomes authenticated.
    /// Catches Application's ValidationException and rethrows with the first
    /// human-readable error message so Login.razor can display it directly.
    /// </summary>
    public async Task<LoginResult> LoginAsync(string email, string password)
    {
        try
        {
            LoginResult result = await _mediator.Send(new LoginCommand(email, password));
            await _authStateProvider.MarkUserAsAuthenticated(result.Token);
            return result;
        }
        catch (AppValidationException ex)
        {
            // Extract the first concrete error from the Errors dictionary.
            // Without this, ex.Message returns the generic "One or more validation
            // failures have occurred." instead of e.g. "Invalid email or password."
            string message = ex.Errors.Values
                .SelectMany(errors => errors)
                .FirstOrDefault() ?? ex.Message;

            throw new InvalidOperationException(message);
        }
    }

    /// <summary>
    /// Sends RegisterCommand to create a new Domain AppUser and returns the new Guid.
    /// Registration does not automatically log the user in — the caller should
    /// redirect to /login after success.
    /// Catches ValidationException and surfaces the first field error to the UI.
    /// </summary>
    public async Task<Guid> RegisterAsync(
        string firstName, string lastName, string email, string password)
    {
        try
        {
            return await _mediator.Send(new RegisterCommand(firstName, lastName, email, password));
        }
        catch (AppValidationException ex)
        {
            string message = ex.Errors.Values
                .SelectMany(errors => errors)
                .FirstOrDefault() ?? ex.Message;

            throw new InvalidOperationException(message);
        }
    }

    /// <summary>
    /// Removes the JWT from client storage and transitions the circuit to the
    /// anonymous state, triggering immediate redirect to /login for protected routes.
    /// </summary>
    public async Task LogoutAsync()
    {
        await _authStateProvider.MarkUserAsLoggedOut();
    }
}
