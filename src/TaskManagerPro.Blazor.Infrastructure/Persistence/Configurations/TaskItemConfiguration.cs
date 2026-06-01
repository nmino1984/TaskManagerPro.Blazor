using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagerPro.Blazor.Domain.Entities;
using TaskManagerPro.Blazor.Domain.Enums;

namespace TaskManagerPro.Blazor.Infrastructure.Persistence.Configurations;

/// <summary>
/// Fluent API configuration for the TaskItem aggregate root.
/// Enums are stored as strings so database records remain readable
/// without needing to consult the codebase for numeric mappings.
/// Backing fields are declared explicitly so EF Core can hydrate
/// readonly properties without any setter visibility on the Domain entity.
/// </summary>
public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.ToTable("Tasks");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Description)
            .HasMaxLength(500);

        builder.Property(t => t.Priority)
            .HasConversion(
                p => p.ToString(),
                p => Enum.Parse<TaskPriority>(p))
            .HasMaxLength(20);

        builder.Property(t => t.Status)
            .HasField("_status")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasConversion(
                s => s.ToString(),
                s => Enum.Parse<WorkTaskStatus>(s))
            .HasMaxLength(20);

        builder.Property(t => t.DueDate)
            .HasField("_dueDate")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasOne(t => t.User)
            .WithMany(u => u.Tasks)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(t => t.SubTasks)
            .WithOne(s => s.TaskItem)
            .HasForeignKey(s => s.TaskItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.Milestones)
            .WithOne(m => m.TaskItem)
            .HasForeignKey(m => m.TaskItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.Notifications)
            .WithOne(n => n.TaskItem)
            .HasForeignKey(n => n.TaskItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(t => t.UserId);
        builder.HasIndex(t => t.IsDeleted);
    }
}
