using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagerPro.Blazor.Domain.Entities;

namespace TaskManagerPro.Blazor.Infrastructure.Persistence.Configurations;

/// <summary>
/// Fluent API configuration for AppUser (Domain user).
/// Stored in a separate table from AspNetUsers (Identity) to preserve
/// the clean boundary between Domain and Infrastructure concerns.
/// </summary>
public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.ToTable("AppUsers");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(u => u.PasswordHash)
            .IsRequired();

        builder.HasIndex(u => u.Email)
            .IsUnique();
    }
}
