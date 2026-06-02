using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace TaskManagerPro.Blazor.Web.Pages;

/// <summary>
/// Base class for all pages that require an authenticated user.
/// Checking auth in OnAfterRenderAsync guarantees we are in the
/// interactive Blazor Server circuit where JS interop (and therefore
/// localStorage) is available. This avoids the SSR pre-render phase
/// where CustomAuthStateProvider returns anonymous and triggers a 404.
/// </summary>
public abstract class AuthenticatedPageBase : ComponentBase
{
    [Inject] protected AuthenticationStateProvider AuthStateProvider { get; set; } = null!;
    [Inject] protected NavigationManager Navigation { get; set; } = null!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;

        AuthenticationState state = await AuthStateProvider.GetAuthenticationStateAsync();

        if (state.User.Identity?.IsAuthenticated != true)
        {
            string returnUrl = Uri.EscapeDataString(Navigation.Uri);
            Navigation.NavigateTo($"/login?returnUrl={returnUrl}");
        }
    }
}
