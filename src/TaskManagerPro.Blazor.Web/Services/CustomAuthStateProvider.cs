using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace TaskManagerPro.Blazor.Web.Services;

/// <summary>
/// Blazor-aware AuthenticationStateProvider that stores the JWT in localStorage
/// and rebuilds the ClaimsPrincipal from it on every auth-state query.
/// Keeping auth state in localStorage means the user stays logged in across
/// Blazor circuit reconnections without a server-side session store.
/// </summary>
public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly IJSRuntime _jsRuntime;
    private static readonly AuthenticationState _anonymous =
        new(new ClaimsPrincipal(new ClaimsIdentity()));

    private const string StorageKey = "authToken";

    /// <summary>
    /// Receives IJSRuntime via DI. Scoped lifetime ensures one instance per Blazor circuit.
    /// </summary>
    public CustomAuthStateProvider(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    /// <summary>
    /// Called by Blazor's CascadingAuthenticationState on every render that needs
    /// auth context. Reads the token from localStorage and decodes it into a
    /// ClaimsPrincipal so [Authorize] attributes and AuthorizeView work correctly.
    /// Returns an anonymous state when no valid token is present or when
    /// JS interop is unavailable during server-side prerendering.
    /// </summary>
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            string? token = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", StorageKey);

            if (string.IsNullOrWhiteSpace(token))
                return _anonymous;

            ClaimsPrincipal user = BuildClaimsPrincipal(token);
            return new AuthenticationState(user);
        }
        catch
        {
            // JS interop is unavailable during SSR prerendering — return anonymous
            // so the circuit can complete; auth state is re-evaluated after hydration.
            return _anonymous;
        }
    }

    /// <summary>
    /// Persists the JWT in localStorage and immediately notifies all subscribers
    /// so the UI reacts (e.g. hides Login button, shows user name) without a page reload.
    /// Called by AuthService after a successful login response.
    /// </summary>
    public async Task MarkUserAsAuthenticated(string token)
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", StorageKey, token);
        ClaimsPrincipal user = BuildClaimsPrincipal(token);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    /// <summary>
    /// Removes the JWT from localStorage and notifies subscribers with an anonymous
    /// principal so protected routes redirect to /login immediately.
    /// Called by AuthService on explicit logout.
    /// </summary>
    public async Task MarkUserAsLoggedOut()
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", StorageKey);
        NotifyAuthenticationStateChanged(Task.FromResult(_anonymous));
    }

    /// <summary>
    /// Decodes the JWT and wraps its claims in a ClaimsPrincipal.
    /// The authentication type "jwt" marks the identity as authenticated,
    /// which is what [Authorize] checks via IsAuthenticated.
    /// </summary>
    private static ClaimsPrincipal BuildClaimsPrincipal(string token)
    {
        JwtSecurityTokenHandler handler = new();
        JwtSecurityToken jwt = handler.ReadJwtToken(token);

        if (jwt.ValidTo != DateTime.MinValue && jwt.ValidTo < DateTime.UtcNow)
            return new ClaimsPrincipal(new ClaimsIdentity());

        ClaimsIdentity identity = new(jwt.Claims, "jwt");
        return new ClaimsPrincipal(identity);
    }
}
