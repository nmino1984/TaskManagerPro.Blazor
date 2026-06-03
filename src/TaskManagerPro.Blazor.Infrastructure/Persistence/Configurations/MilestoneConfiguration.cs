using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagerPro.Blazor.Domain.Entities;
using TaskManagerPro.Blazor.Domain.Enums;

namespace TaskManagerPro.Blazor.Infrastructure.Persistence.Configurations;

/// <summary>
/// Status and TargetDate are mapped via backing fields so EF Core can hydrate readonly
/// properties. Status is stored as a string so Overdue transitions by background jobs
/// can be inspected directly in the database without decoding integers.
/// </summary>
public class MilestoneConfiguration : IEntityTypeConfiguration<Milestone>
{
    public void Configure(EntityTypeBuilder<Milestone> builder)
    {
        builder.ToTable("Milestones");
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Title).IsRequired().HasMaxLength(200);
        builder.Property(m => m.Description).HasMaxLength(500);

        builder.Property(m => m.TargetDate)
            .HasField("_targetDate")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Property(m => m.Status)
            .HasField("_status")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasConversion(s => s.ToString(), s => Enum.Parse<MilestoneStatus>(s))
            .HasMaxLength(20);

        builder.HasIndex(m => m.TaskItemId);
    }
}
