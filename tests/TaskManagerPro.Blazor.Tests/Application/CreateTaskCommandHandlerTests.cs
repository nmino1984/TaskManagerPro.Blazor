using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using TaskManagerPro.Blazor.Application.Features.Tasks.Commands.CreateTask;
using TaskManagerPro.Blazor.Domain.Entities;
using TaskManagerPro.Blazor.Domain.Enums;
using TaskManagerPro.Blazor.Domain.Interfaces;

namespace TaskManagerPro.Blazor.Tests.Application;

public class CreateTaskCommandHandlerTests
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<TaskItem> _tasksRepo;
    private readonly IRepository<Notification> _notificationsRepo;
    private readonly CreateTaskCommandHandler _handler;

    public CreateTaskCommandHandlerTests()
    {
        _unitOfWork        = Substitute.For<IUnitOfWork>();
        _tasksRepo         = Substitute.For<IRepository<TaskItem>>();
        _notificationsRepo = Substitute.For<IRepository<Notification>>();

        _unitOfWork.Tasks.Returns(_tasksRepo);
        _unitOfWork.Notifications.Returns(_notificationsRepo);

        _handler = new CreateTaskCommandHandler(_unitOfWork, Substitute.For<ILogger<CreateTaskCommandHandler>>());
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesTaskAndReturnsId()
    {
        var userId  = Guid.NewGuid();
        var command = new CreateTaskCommand("My Task", "Desc", null, TaskPriority.Medium, userId);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBe(Guid.Empty);
        await _tasksRepo.Received(1).AddAsync(Arg.Any<TaskItem>(), Arg.Any<CancellationToken>());
        await _notificationsRepo.Received(1).AddAsync(
            Arg.Is<Notification>(n => n.Title == "Task created"),
            Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ValidCommandWithAssignedUser_CreatesNotificationForAssignee()
    {
        var userId     = Guid.NewGuid();
        var assigneeId = Guid.NewGuid();
        var command    = new CreateTaskCommand("Assigned Task", "Desc", null, TaskPriority.High, userId, assigneeId);

        await _handler.Handle(command, CancellationToken.None);

        // One for creator ("Task created") + one for assignee ("New task assigned to you")
        await _notificationsRepo.Received(2).AddAsync(Arg.Any<Notification>(), Arg.Any<CancellationToken>());
        await _notificationsRepo.Received(1).AddAsync(
            Arg.Is<Notification>(n => n.Title == "New task assigned to you" && n.UserId == assigneeId),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ValidCommandAssignedToSelf_DoesNotCreateAssignmentNotification()
    {
        var userId  = Guid.NewGuid();
        var command = new CreateTaskCommand("Self Task", "Desc", null, TaskPriority.Low, userId, userId);

        await _handler.Handle(command, CancellationToken.None);

        // Only the "Task created" notification — no assignment notification when creator == assignee
        await _notificationsRepo.Received(1).AddAsync(Arg.Any<Notification>(), Arg.Any<CancellationToken>());
        await _notificationsRepo.Received(1).AddAsync(
            Arg.Is<Notification>(n => n.Title == "Task created"),
            Arg.Any<CancellationToken>());
    }
}
