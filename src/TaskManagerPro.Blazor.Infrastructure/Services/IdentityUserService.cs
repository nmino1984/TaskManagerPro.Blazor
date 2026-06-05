using Microsoft.AspNetCore.Identity;
using TaskManagerPro.Blazor.Application.Common.Interfaces;
using TaskManagerPro.Blazor.Infrastructure.Identity;

namespace TaskManagerPro.Blazor.Infrastructure.Services;

public class IdentityUserService : IIdentityUserService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public IdentityUserService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task UpdateUserAsync(Guid userId, string email, string firstName, string lastName, CancellationToken ct)
    {
        var identityUser = await _userManager.FindByIdAsync(userId.ToString());
        if (identityUser is null) return;

        identityUser.Email     = email;
        identityUser.UserName  = email;
        identityUser.FirstName = firstName;
        identityUser.LastName  = lastName;

        var result = await _userManager.UpdateAsync(identityUser);
        if (!result.Succeeded)
            throw new Application.Common.Exceptions.ValidationException(
                result.Errors.ToDictionary(e => e.Code, e => new[] { e.Description }));
    }

    public async Task ResetPasswordAsync(Guid userId, string newPassword, CancellationToken ct)
    {
        var identityUser = await _userManager.FindByIdAsync(userId.ToString());
        if (identityUser is null) return;

        await _userManager.RemovePasswordAsync(identityUser);
        var result = await _userManager.AddPasswordAsync(identityUser, newPassword);

        if (!result.Succeeded)
            throw new Application.Common.Exceptions.ValidationException(
                result.Errors.ToDictionary(e => e.Code, e => new[] { e.Description }));
    }
}
