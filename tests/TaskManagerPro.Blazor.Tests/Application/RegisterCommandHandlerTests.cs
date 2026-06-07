using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using TaskManagerPro.Blazor.Application.Common.Exceptions;
using TaskManagerPro.Blazor.Application.Common.Interfaces;
using TaskManagerPro.Blazor.Application.Features.Auth.Commands.Register;
using TaskManagerPro.Blazor.Domain.Entities;
using TaskManagerPro.Blazor.Domain.Interfaces;

namespace TaskManagerPro.Blazor.Tests.Application;

public class RegisterCommandHandlerTests
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<AppUser> _usersRepo;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUserRegistrationService _registrationService;
    private readonly IEmailService _emailService;
    private readonly IEmailVerificationSettings _verificationSettings;
    private readonly RegisterCommandHandler _handler;

    public RegisterCommandHandlerTests()
    {
        _unitOfWork           = Substitute.For<IUnitOfWork>();
        _usersRepo            = Substitute.For<IRepository<AppUser>>();
        _passwordHasher       = Substitute.For<IPasswordHasher>();
        _registrationService  = Substitute.For<IUserRegistrationService>();
        _emailService         = Substitute.For<IEmailService>();
        _verificationSettings = Substitute.For<IEmailVerificationSettings>();

        _unitOfWork.Users.Returns(_usersRepo);
        _passwordHasher.Hash(Arg.Any<string>()).Returns("hashed_password");
        _verificationSettings.ExpirationMinutes.Returns(60);
        _verificationSettings.BaseUrl.Returns("http://localhost:5026");

        _handler = new RegisterCommandHandler(_unitOfWork, _passwordHasher, _registrationService, _emailService,
            _verificationSettings, Substitute.For<ILogger<RegisterCommandHandler>>());
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesUserAndReturnsId()
    {
        _usersRepo.FindAsync(Arg.Any<System.Linq.Expressions.Expression<Func<AppUser, bool>>>(),
                             Arg.Any<CancellationToken>())
                  .Returns(new List<AppUser>());

        var command = new RegisterCommand("Alice", "Wonder", "alice@test.com", "Password1!");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBe(Guid.Empty);
        await _usersRepo.Received(1).AddAsync(
            Arg.Is<AppUser>(u => u.Email == "alice@test.com" && u.FirstName == "Alice"),
            Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        await _registrationService.Received(1).CreateIdentityUserAsync(
            Arg.Any<Guid>(), "Alice", "Wonder", "alice@test.com", "Password1!", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_DuplicateEmail_ThrowsValidationException()
    {
        var existing = new AppUser("Existing", "User", "taken@test.com", "hashed");

        _usersRepo.FindAsync(Arg.Any<System.Linq.Expressions.Expression<Func<AppUser, bool>>>(),
                             Arg.Any<CancellationToken>())
                  .Returns(new List<AppUser> { existing });

        var command = new RegisterCommand("New", "User", "taken@test.com", "Password1!");

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>()
                 .WithMessage("*validation*");

        await _usersRepo.DidNotReceive().AddAsync(Arg.Any<AppUser>(), Arg.Any<CancellationToken>());
    }
}
