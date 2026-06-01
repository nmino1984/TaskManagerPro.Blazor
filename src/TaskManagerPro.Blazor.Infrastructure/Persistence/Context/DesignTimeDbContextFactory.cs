using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TaskManagerPro.Blazor.Infrastructure.Persistence.Context;

/// <summary>
/// Provides a DbContext instance to EF Core Tools at design time (migrations, scaffolding).
/// Only used by the CLI — never instantiated at runtime. The connection string here
/// points to the local Docker container and must not be used in any other environment.
/// </summary>
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    /// <summary>
    /// Creates the DbContext with a hardcoded local connection string so
    /// 'dotnet ef migrations add' can run without a running ASP.NET host.
    /// </summary>
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<ApplicationDbContext> optionsBuilder = new();

        optionsBuilder.UseSqlServer(
            "Server=localhost,1433;Database=TaskManagerProDb;User Id=sa;Password=Admin1234!;TrustServerCertificate=True;",
            sqlOptions => sqlOptions.MigrationsAssembly(
                typeof(ApplicationDbContext).Assembly.FullName));

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
