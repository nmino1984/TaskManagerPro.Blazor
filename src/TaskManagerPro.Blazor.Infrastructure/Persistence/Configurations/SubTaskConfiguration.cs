using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagerPro.Blazor.Domain.Entities;

namespace TaskManagerPro.Blazor.Infrastructure.Persistence.Configurations;

// IsCompleted is mapped via its backing field so EF Core can hydrate the readonly property
public class SubTaskConfiguration : IEntityTypeConfiguration<SubTask>
{
    public void Configure(EntityTypeBuilder<SubTask> builder)
    {
        builder.ToTable("SubTasks");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Title).IsRequired().HasMaxLength(200);
        builder.Property(s => s.Description).HasMaxLength(500);

        builder.Property(s => s.IsCompleted)
            .HasField("_isCompleted")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(s => s.TaskItemId);
    }
}
