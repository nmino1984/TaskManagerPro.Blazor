using FluentAssertions;
using TaskManagerPro.Blazor.Domain.Entities;
using TaskManagerPro.Blazor.Domain.Enums;

namespace TaskManagerPro.Blazor.Tests.Domain;

public class MilestoneTests
{
    [Fact]
    public void Constructor_SetsCorrectValues()
    {
        var taskId = Guid.NewGuid();
        var target = DateTime.UtcNow.AddDays(5);

        var milestone = new Milestone("Launch", "Ship it", target, taskId);

        milestone.Title.Should().Be("Launch");
        milestone.Description.Should().Be("Ship it");
        milestone.TargetDate.Should().Be(target);
        milestone.TaskItemId.Should().Be(taskId);
        milestone.Status.Should().Be(MilestoneStatus.Pending);
    }

    [Fact]
    public void Update_WithCompletedStatus_ChangesStatus()
    {
        var taskId = Guid.NewGuid();
        var milestone = new Milestone("v1.0", "First release", DateTime.UtcNow.AddDays(10), taskId);

        milestone.Update("v1.0 Final", "Final release", DateTime.UtcNow.AddDays(12), MilestoneStatus.Completed);

        milestone.Status.Should().Be(MilestoneStatus.Completed);
        milestone.Title.Should().Be("v1.0 Final");
    }

    [Fact]
    public void Update_WithOverdueStatus_ChangesStatus()
    {
        var taskId = Guid.NewGuid();
        var milestone = new Milestone("Beta", "Beta release", DateTime.UtcNow.AddDays(-1), taskId);

        milestone.Update(milestone.Title, milestone.Description, milestone.TargetDate, MilestoneStatus.Overdue);

        milestone.Status.Should().Be(MilestoneStatus.Overdue);
    }

    [Fact]
    public void Update_ChangesAllProperties()
    {
        var taskId = Guid.NewGuid();
        var original = DateTime.UtcNow.AddDays(5);
        var updated  = DateTime.UtcNow.AddDays(10);
        var milestone = new Milestone("Old Title", "Old desc", original, taskId);

        milestone.Update("New Title", "New desc", updated, MilestoneStatus.InProgress);

        milestone.Title.Should().Be("New Title");
        milestone.Description.Should().Be("New desc");
        milestone.TargetDate.Should().Be(updated);
        milestone.Status.Should().Be(MilestoneStatus.InProgress);
    }
}
