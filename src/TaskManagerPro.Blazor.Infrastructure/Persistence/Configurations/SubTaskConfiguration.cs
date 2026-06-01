using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagerPro.Blazor.Domain.Entities;

namespace TaskManagerPro.Blazor.Infrastructure.Persistence.Configurations;

/// <summary>
/// Fluent API configuration for SubTask.
/// IsCompleted is mapped via its backing field so EF Core can hydrate
/// the readonly property without a public or internal setter on the entity.
/// </summary>
public class SubTaskConfiguration : IEntityTypeConfiguration<SubTask>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<SubTask> builder)
    {
        builder.ToTable("SubTasks");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Description)
            .HasMaxLength(500);

        builder.Property(s => s.IsCompleted)
            .HasField("_isCompleted")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(s => s.TaskItemId);
    }
}
