using FluentAssertions;
using TaskManagerPro.Blazor.Domain.Entities;
using TaskManagerPro.Blazor.Domain.Enums;

namespace TaskManagerPro.Blazor.Tests.Domain;

public class TaskItemTests
{
    private static TaskItem CreateTask(
        string title = "Test Task",
        string description = "Description",
        TaskPriority priority = TaskPriority.Medium,
        Guid? userId = null,
        Guid? assignedToUserId = null)
        => new(title, description, null, priority, userId ?? Guid.NewGuid(), assignedToUserId);

    [Fact]
    public void Constructor_SetsCorrectInitialValues()
    {
        var userId = Guid.NewGuid();
        var assignedId = Guid.NewGuid();
        var due = DateTime.UtcNow.AddDays(3);

        var task = new TaskItem("My Task", "Do stuff", due, TaskPriority.High, userId, assignedId);

        task.Title.Should().Be("My Task");
        task.Description.Should().Be("Do stuff");
        task.DueDate.Should().Be(due);
        task.Priority.Should().Be(TaskPriority.High);
        task.UserId.Should().Be(userId);
        task.AssignedToUserId.Should().Be(assignedId);
        task.Status.Should().Be(WorkTaskStatus.Pending);
    }

    [Fact]
    public void StartProgress_WhenPending_ChangesStatusToInProgress()
    {
        var task = CreateTask();

        task.StartProgress();

        task.Status.Should().Be(WorkTaskStatus.InProgress);
    }

    [Fact]
    public void StartProgress_WhenCompleted_ThrowsInvalidOperationException()
    {
        var task = CreateTask();
        task.StartProgress();
        task.Complete();

        Action act = () => task.StartProgress();

        act.Should().Throw<InvalidOperationException>()
           .WithMessage("*Completed*");
    }

    [Fact]
    public void StartProgress_WhenCancelled_ThrowsInvalidOperationException()
    {
        var task = CreateTask();
        task.Cancel();

        Action act = () => task.StartProgress();

        act.Should().Throw<InvalidOperationException>()
           .WithMessage("*Cancelled*");
    }

    [Fact]
    public void Complete_WhenInProgress_ChangesStatusToCompleted()
    {
        var task = CreateTask();
        task.StartProgress();

        task.Complete();

        task.Status.Should().Be(WorkTaskStatus.Completed);
    }

    [Fact]
    public void Complete_WhenCancelled_ThrowsInvalidOperationException()
    {
        var task = CreateTask();
        task.Cancel();

        Action act = () => task.Complete();

        act.Should().Throw<InvalidOperationException>()
           .WithMessage("*cancelled*");
    }

    [Fact]
    public void Cancel_WhenPending_ChangesStatusToCancelled()
    {
        var task = CreateTask();

        task.Cancel();

        task.Status.Should().Be(WorkTaskStatus.Cancelled);
    }

    [Fact]
    public void Cancel_WhenCompleted_ThrowsInvalidOperationException()
    {
        var task = CreateTask();
        task.StartProgress();
        task.Complete();

        Action act = () => task.Cancel();

        act.Should().Throw<InvalidOperationException>()
           .WithMessage("*completed*");
    }

    [Theory]
    [InlineData(WorkTaskStatus.InProgress)]
    [InlineData(WorkTaskStatus.Completed)]
    [InlineData(WorkTaskStatus.Cancelled)]
    public void ResetToPending_FromAnyStatus_ChangesStatusToPending(WorkTaskStatus startStatus)
    {
        var task = CreateTask();
        if (startStatus == WorkTaskStatus.InProgress) task.StartProgress();
        if (startStatus == WorkTaskStatus.Completed)  { task.StartProgress(); task.Complete(); }
        if (startStatus == WorkTaskStatus.Cancelled)  task.Cancel();

        task.ResetToPending();

        task.Status.Should().Be(WorkTaskStatus.Pending);
    }

    [Fact]
    public void Update_ChangesCorrectProperties()
    {
        var task = CreateTask();
        var newAssignee = Guid.NewGuid();
        var newDue = DateTime.UtcNow.AddDays(7);

        task.Update("Updated Title", "Updated Desc", newDue, TaskPriority.Critical, newAssignee);

        task.Title.Should().Be("Updated Title");
        task.Description.Should().Be("Updated Desc");
        task.DueDate.Should().Be(newDue);
        task.Priority.Should().Be(TaskPriority.Critical);
        task.AssignedToUserId.Should().Be(newAssignee);
    }
}
