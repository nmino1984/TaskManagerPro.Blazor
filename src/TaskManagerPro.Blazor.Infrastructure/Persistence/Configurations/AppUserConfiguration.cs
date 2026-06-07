using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagerPro.Blazor.Domain.Entities;

namespace TaskManagerPro.Blazor.Infrastructure.Persistence.Configurations;

// Separate table from AspNetUsers to preserve the Domain/Infrastructure boundary
public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.ToTable("AppUsers");
        builder.HasKey(u => u.Id);

        builder.Property(u => u.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(u => u.LastName).IsRequired().HasMaxLength(100);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(256);
        builder.Property(u => u.PasswordHash).IsRequired();

        // Backing fields — EF maps these via the read-only property expression
        builder.Property(u => u.IsEmailVerified).HasField("_isEmailVerified").HasDefaultValue(false);
        builder.Property(u => u.VerificationToken).HasField("_verificationToken").HasMaxLength(64);
        builder.Property(u => u.VerificationTokenExpiry).HasField("_verificationTokenExpiry");

        builder.HasIndex(u => u.Email).IsUnique();
    }
}
