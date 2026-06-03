using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace TaskManagerPro.Blazor.Web.Services;

/// <summary>
/// Stores the JWT in localStorage so the user stays logged in across Blazor
/// circuit reconnections without needing a server-side session store.
/// </summary>
public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly IJSRuntime _jsRuntime;
    private static readonly AuthenticationState _anonymous =
        new(new ClaimsPrincipal(new ClaimsIdentity()));

    private const string StorageKey = "authToken";

    public CustomAuthStateProvider(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var token = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", StorageKey);

            if (string.IsNullOrWhiteSpace(token))
                return _anonymous;

            return new AuthenticationState(BuildClaimsPrincipal(token));
        }
        catch
        {
            // JS interop is unavailable during SSR prerendering — return anonymous so the
            // circuit can complete; auth state is re-evaluated after hydration
            return _anonymous;
        }
    }

    // Persists the token and notifies subscribers so the UI reacts without a page reload
    public async Task MarkUserAsAuthenticated(string token)
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", StorageKey, token);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(BuildClaimsPrincipal(token))));
    }

    // Removes the token and notifies subscribers so protected routes redirect to /login immediately
    public async Task MarkUserAsLoggedOut()
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", StorageKey);
        NotifyAuthenticationStateChanged(Task.FromResult(_anonymous));
    }

    private static ClaimsPrincipal BuildClaimsPrincipal(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        if (jwt.ValidTo != DateTime.MinValue && jwt.ValidTo < DateTime.UtcNow)
            return new ClaimsPrincipal(new ClaimsIdentity());

        // The "jwt" auth type marks the identity as authenticated — what [Authorize] checks via IsAuthenticated
        return new ClaimsPrincipal(new ClaimsIdentity(jwt.Claims, "jwt"));
    }
}
