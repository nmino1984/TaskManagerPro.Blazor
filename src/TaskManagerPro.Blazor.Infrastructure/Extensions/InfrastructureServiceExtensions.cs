using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskManagerPro.Blazor.Domain.Interfaces;
using TaskManagerPro.Blazor.Infrastructure.Identity;
using TaskManagerPro.Blazor.Infrastructure.Persistence.Context;
using TaskManagerPro.Blazor.Infrastructure.Persistence.Repositories;

namespace TaskManagerPro.Blazor.Infrastructure.Extensions;

/// <summary>
/// Extension method that registers all Infrastructure services with the DI container.
/// Centralising registration here keeps Program.cs clean and ensures the Web project
/// only needs a single call to wire up the entire data access layer.
/// </summary>
public static class InfrastructureServiceExtensions
{
    /// <summary>
    /// Registers ApplicationDbContext with SQL Server, ASP.NET Identity, and the
    /// Unit of Work pattern. Connection string is read from the "DefaultConnection" key.
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

        services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        return services;
    }
}
