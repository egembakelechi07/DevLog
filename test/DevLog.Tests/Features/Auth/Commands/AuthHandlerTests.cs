using DevLog.Application.Features.Auth.Commands.LoginUser;
using DevLog.Application.Features.Auth.Commands.RegisterUser;
using DevLog.Domain.Entities;
using DevLog.Domain.Interfaces;
using DevLog.Shared.Exceptions;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Sdk;

namespace DevLog.Tests.Features.Auth.Commands;

public class AuthHandlerTests
{
    private readonly Mock<IUserRepository> _mockUserRepo;
    private readonly Mock<IJwtService> _mockJwtService;
    private readonly Mock<ILogger<LoginUserCommandHandler>> _mockLogger;
    private readonly LoginUserCommandHandler _handler;

    public AuthHandlerTests()
    {
        _mockUserRepo = new Mock<IUserRepository>();
        _mockJwtService = new Mock<IJwtService>();
        _mockLogger = new Mock<ILogger<LoginUserCommandHandler>>();
        _handler = new LoginUserCommandHandler(_mockJwtService.Object,_mockUserRepo.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_EmailIsEmpty()
    {
        var command = new LoginUserCommand(
            email: "",
            password: "Test123$",
            rememberMe: true);

        var result = await _handler.Handle(command,CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_UserNotFound()
    {
        _mockUserRepo
            .Setup(r => r.GetByEmailAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync((User?) null);

        var command = new LoginUserCommand(
            email: "notfound@devlog.com",
            password: "Test1234!",
            rememberMe: true
        );

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should(). BeFalse();
        result.Errors.Should().Contain("Invalid Email or Password");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_PasswwordIsInvalid()
    {
        _mockUserRepo
            .Setup(r => r.GetByEmailAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(new User
            {
                Id = Guid.NewGuid(),
                Name = "Dev",
                Email = "Dev@devlog.com",
                PasswordHash = "hashedpassword",
                LastUpdatedAt = DateTime.UtcNow
            });

        _mockUserRepo
            .Setup(r => r.IsValidPassword(
                It.IsAny<string>(),
                It.IsAny<string>()
            )).Returns(false);

            var command = new LoginUserCommand(
            email: "Dev@devlog.com",
            password: "Test1234",
            rememberMe: true
        );

         var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should(). BeFalse();
        result.Errors.Should().Contain("Invalid Email or password");

    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_CredentialsAreValid()
    {

        var userId = Guid.NewGuid();
         _mockUserRepo
            .Setup(r => r.GetByEmailAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(new User
            {
                Id = userId,
                Name = "Dev",
                Email = "Dev@devlog.com",
                PasswordHash = "hashedpassword",
                LastUpdatedAt = DateTime.UtcNow
            });

        _mockUserRepo
            .Setup(r => r.IsValidPassword(
                It.IsAny<string>(),
                It.IsAny<string>()
            )).Returns(true);

        _mockJwtService
            .Setup(r => r.GenerateToken(
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<string>()
            )).Returns("fake.jwt.Token");
        
        _mockJwtService
            .Setup(r => r.GenerateRefreshToken(
            )).Returns("fake.jwt.Token");

            var command = new LoginUserCommand(
            email: "Dev@devlog.com",
            password: "Test1234!",
            rememberMe: true
        );

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should(). BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.AccessToken.Should().NotBeNull();

    }
}