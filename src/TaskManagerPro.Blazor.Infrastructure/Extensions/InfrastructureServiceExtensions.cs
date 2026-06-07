using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskManagerPro.Blazor.Application.Common.Behaviors;
using TaskManagerPro.Blazor.Application.Common.Interfaces;
using TaskManagerPro.Blazor.Domain.Interfaces;
using TaskManagerPro.Blazor.Infrastructure.BackgroundJobs;
using TaskManagerPro.Blazor.Infrastructure.Identity;
using TaskManagerPro.Blazor.Infrastructure.Persistence.Context;
using TaskManagerPro.Blazor.Infrastructure.Persistence.Repositories;
using TaskManagerPro.Blazor.Infrastructure.Services;

namespace TaskManagerPro.Blazor.Infrastructure.Extensions;

/// <summary>
/// Registers all Infrastructure services with the DI container.
/// One call from Program.cs wires up the entire data access layer.
/// </summary>
public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sql => sql.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        // AddIdentity (not AddIdentityCore) registers RoleManager, SignInManager, and
        // default token providers needed for password reset and email confirmation
        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit           = true;
                options.Password.RequiredLength         = 8;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase       = true;
                options.User.RequireUniqueEmail         = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IPasswordHasher, PasswordHasherService>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGeneratorService>();
        services.AddScoped<IUserRegistrationService, UserRegistrationService>();
        services.AddScoped<IIdentityUserService, IdentityUserService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IPhotoService, CloudinaryPhotoService>();
        services.AddSingleton<IEmailVerificationSettings, EmailVerificationSettings>();

        // Open-generic registration runs all FluentValidation validators before each handler
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.AddHostedService<OverdueNotificationJob>();

        return services;
    }
}
