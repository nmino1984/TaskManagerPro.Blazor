using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using TaskManagerPro.Blazor.Application.Common.Exceptions;
using TaskManagerPro.Blazor.Application.Common.Interfaces;
using TaskManagerPro.Blazor.Application.Features.Auth.Commands.Login;
using TaskManagerPro.Blazor.Domain.Entities;
using TaskManagerPro.Blazor.Domain.Interfaces;

namespace TaskManagerPro.Blazor.Tests.Application;

public class LoginCommandHandlerTests
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<AppUser> _usersRepo;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtGenerator;
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _unitOfWork     = Substitute.For<IUnitOfWork>();
        _usersRepo      = Substitute.For<IRepository<AppUser>>();
        _passwordHasher = Substitute.For<IPasswordHasher>();
        _jwtGenerator   = Substitute.For<IJwtTokenGenerator>();

        _unitOfWork.Users.Returns(_usersRepo);

        _handler = new LoginCommandHandler(_unitOfWork, _passwordHasher, _jwtGenerator,
            Substitute.For<ILogger<LoginCommandHandler>>());
    }

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsLoginResult()
    {
        var userId  = Guid.NewGuid();
        var user    = new AppUser("Jane", "Doe", "jane@test.com", "hashed");
        var expires = DateTime.UtcNow.AddHours(1);

        _usersRepo.FindAsync(Arg.Any<System.Linq.Expressions.Expression<Func<AppUser, bool>>>(),
                             Arg.Any<CancellationToken>())
                  .Returns(new List<AppUser> { user });

        _passwordHasher.Verify("secret", user.PasswordHash).Returns(true);
        _jwtGenerator.GenerateToken(Arg.Any<Guid>(), user.Email, user.FirstName, user.LastName, Arg.Any<bool>())
                     .Returns(("tok_abc", expires));

        var result = await _handler.Handle(new LoginCommand("jane@test.com", "secret"), CancellationToken.None);

        result.Token.Should().Be("tok_abc");
        result.Email.Should().Be("jane@test.com");
        result.ExpiresAt.Should().Be(expires);
    }

    [Fact]
    public async Task Handle_UserNotFound_ThrowsValidationException()
    {
        _usersRepo.FindAsync(Arg.Any<System.Linq.Expressions.Expression<Func<AppUser, bool>>>(),
                             Arg.Any<CancellationToken>())
                  .Returns(new List<AppUser>());

        var act = () => _handler.Handle(new LoginCommand("nobody@test.com", "pw"), CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>()
                 .WithMessage("*validation*");
    }

    [Fact]
    public async Task Handle_WrongPassword_ThrowsValidationException()
    {
        var user = new AppUser("Bob", "Smith", "bob@test.com", "hashed");

        _usersRepo.FindAsync(Arg.Any<System.Linq.Expressions.Expression<Func<AppUser, bool>>>(),
                             Arg.Any<CancellationToken>())
                  .Returns(new List<AppUser> { user });

        _passwordHasher.Verify(Arg.Any<string>(), user.PasswordHash).Returns(false);

        var act = () => _handler.Handle(new LoginCommand("bob@test.com", "wrong"), CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
    }
}
