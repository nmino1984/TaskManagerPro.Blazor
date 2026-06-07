using Microsoft.AspNetCore.Identity;
using TaskManagerPro.Blazor.Application.Common.Interfaces;
using TaskManagerPro.Blazor.Infrastructure.Identity;

namespace TaskManagerPro.Blazor.Infrastructure.Services;

public class UserRegistrationService : IUserRegistrationService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserRegistrationService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task CreateIdentityUserAsync(Guid userId, string firstName, string lastName, string email, string password, CancellationToken ct)
    {
        var identityUser = new ApplicationUser
        {
            Id = userId.ToString(),
            UserName = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName
        };

        var result = await _userManager.CreateAsync(identityUser, password);

        if (!result.Succeeded)
        {
            var errors = result.Errors.ToDictionary(
                e => e.Code,
                e => new[] { e.Description });

            throw new Application.Common.Exceptions.ValidationException(errors);
        }
    }
}
