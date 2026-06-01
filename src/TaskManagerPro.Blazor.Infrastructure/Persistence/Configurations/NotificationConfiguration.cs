using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagerPro.Blazor.Domain.Entities;
using TaskManagerPro.Blazor.Domain.Enums;

namespace TaskManagerPro.Blazor.Infrastructure.Persistence.Configurations;

/// <summary>
/// Fluent API configuration for Notification.
/// Status is mapped via its backing field so EF Core can hydrate the readonly property.
/// TaskItemId is nullable because some notifications are system-level
/// and not tied to any specific task.
/// </summary>
public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications");

        builder.HasKey(n => n.Id);

        builder.Property(n => n.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(n => n.Message)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(n => n.Status)
            .HasField("_status")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasConversion(
                s => s.ToString(),
                s => Enum.Parse<NotificationStatus>(s))
            .HasMaxLength(20);

        builder.HasOne(n => n.User)
            .WithMany(u => u.Notifications)
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(n => n.UserId);
        builder.HasIndex(n => n.Status);
    }
}
