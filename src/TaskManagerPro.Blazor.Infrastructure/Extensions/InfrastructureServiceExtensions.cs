using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskManagerPro.Blazor.Application.Common.Behaviors;
using TaskManagerPro.Blazor.Application.Common.Interfaces;
using TaskManagerPro.Blazor.Domain.Interfaces;
using TaskManagerPro.Blazor.Infrastructure.Identity;
using TaskManagerPro.Blazor.Infrastructure.Persistence.Context;
using TaskManagerPro.Blazor.Infrastructure.Persistence.Repositories;
using TaskManagerPro.Blazor.Infrastructure.Services;

namespace TaskManagerPro.Blazor.Infrastructure.Extensions;

/// <summary>
/// Extension method that registers all Infrastructure services with the DI container.
/// Centralising registration here keeps Program.cs clean and ensures the Web project
/// only needs a single call to wire up the entire data access layer.
/// </summary>
public static class InfrastructureServiceExtensions
{
    /// <summary>
    /// Registers: ApplicationDbContext with SQL Server, ASP.NET Identity (with roles,
    /// SignInManager and default token providers), Unit of Work, generic Repository,
    /// password hashing, JWT generation, and the MediatR ValidationBehavior pipeline
    /// so every command is validated before its handler runs.
    /// Connection string is read from configuration["ConnectionStrings:DefaultConnection"].
    /// </summary>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sqlOptions => sqlOptions.MigrationsAssembly(
                    typeof(ApplicationDbContext).Assembly.FullName)));

        // AddIdentity (vs AddIdentityCore) registers RoleManager, SignInManager, and
        // default token providers needed for password reset and email confirmation flows.
        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        services.AddScoped<IPasswordHasher, PasswordHasherService>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGeneratorService>();

        // Open-generic registration intercepts every IRequest<TResponse> and runs
        // all registered FluentValidation validators before the handler executes.
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}
