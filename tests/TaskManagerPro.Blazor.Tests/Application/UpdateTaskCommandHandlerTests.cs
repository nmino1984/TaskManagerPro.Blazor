using FluentAssertions;
using NSubstitute;
using TaskManagerPro.Blazor.Application.Common.Exceptions;
using TaskManagerPro.Blazor.Application.Features.Tasks.Commands.UpdateTask;
using TaskManagerPro.Blazor.Domain.Entities;
using TaskManagerPro.Blazor.Domain.Enums;
using TaskManagerPro.Blazor.Domain.Interfaces;

namespace TaskManagerPro.Blazor.Tests.Application;

public class UpdateTaskCommandHandlerTests
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<TaskItem> _tasksRepo;
    private readonly IRepository<Notification> _notificationsRepo;
    private readonly UpdateTaskCommandHandler _handler;

    public UpdateTaskCommandHandlerTests()
    {
        _unitOfWork        = Substitute.For<IUnitOfWork>();
        _tasksRepo         = Substitute.For<IRepository<TaskItem>>();
        _notificationsRepo = Substitute.For<IRepository<Notification>>();

        _unitOfWork.Tasks.Returns(_tasksRepo);
        _unitOfWork.Notifications.Returns(_notificationsRepo);

        _handler = new UpdateTaskCommandHandler(_unitOfWork);
    }

    private static TaskItem BuildTask(Guid? userId = null, Guid? assignedToUserId = null)
        => new("Original Title", "Original Desc", null, TaskPriority.Medium, userId ?? Guid.NewGuid(), assignedToUserId);

    [Fact]
    public async Task Handle_ValidCommand_UpdatesTask()
    {
        var taskId = Guid.NewGuid();
        var task   = BuildTask();
        _tasksRepo.GetByIdAsync(taskId, Arg.Any<CancellationToken>()).Returns(task);

        var command = new UpdateTaskCommand(taskId, "New Title", "New Desc", null, TaskPriority.High);

        await _handler.Handle(command, CancellationToken.None);

        task.Title.Should().Be("New Title");
        task.Priority.Should().Be(TaskPriority.High);
        await _tasksRepo.Received(1).UpdateAsync(task, Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_TaskNotFound_ThrowsNotFoundException()
    {
        var taskId = Guid.NewGuid();
        _tasksRepo.GetByIdAsync(taskId, Arg.Any<CancellationToken>()).Returns((TaskItem?)null);

        var command = new UpdateTaskCommand(taskId, "Title", "Desc", null, TaskPriority.Low);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_StatusChangedToCompleted_CreatesNotification()
    {
        var taskId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var task   = BuildTask(userId);
        task.StartProgress();
        _tasksRepo.GetByIdAsync(taskId, Arg.Any<CancellationToken>()).Returns(task);

        var command = new UpdateTaskCommand(taskId, task.Title, task.Description, null, task.Priority,
                                            Status: WorkTaskStatus.Completed);

        await _handler.Handle(command, CancellationToken.None);

        task.Status.Should().Be(WorkTaskStatus.Completed);
        await _notificationsRepo.Received(1).AddAsync(
            Arg.Is<Notification>(n => n.Title.Contains("completed") && n.UserId == userId),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_AssignedToNewUser_CreatesNotificationForNewAssignee()
    {
        var taskId     = Guid.NewGuid();
        var assigneeId = Guid.NewGuid();
        var task       = BuildTask(); // no initial assignment
        _tasksRepo.GetByIdAsync(taskId, Arg.Any<CancellationToken>()).Returns(task);

        var command = new UpdateTaskCommand(taskId, task.Title, task.Description, null, task.Priority,
                                            AssignedToUserId: assigneeId);

        await _handler.Handle(command, CancellationToken.None);

        await _notificationsRepo.Received(1).AddAsync(
            Arg.Is<Notification>(n => n.Title == "Task assigned to you" && n.UserId == assigneeId),
            Arg.Any<CancellationToken>());
    }
}
