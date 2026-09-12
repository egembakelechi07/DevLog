using DevLog.Application.Features.Logs.Commands.CreateLog;
using DevLog.Domain.Entities;
using DevLog.Domain.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace DevLog.Tests.Features.Logs.Commands;

public class CreateLogCommandHandlerTests
{
    private readonly Mock<ILogRepository> _mockLogRepo;
    private readonly Mock<ITagRepository> _mockTagRepo;
    private readonly Mock<ILogger<CreateLogCommandHandler>> _mockLogger;
    private readonly CreateLogCommandHandler _handler;

    public CreateLogCommandHandlerTests()
    {
        _mockLogRepo = new Mock<ILogRepository>();
        _mockTagRepo = new Mock<ITagRepository>();
        _mockLogger = new Mock<ILogger<CreateLogCommandHandler>>();
        _handler = new CreateLogCommandHandler(_mockLogRepo.Object, _mockTagRepo.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_TitleIsEmpty()
    {
        var command = new CreateLogCommand(
            UserId: Guid.NewGuid(),
            Title: "",
            Description: "Test Desc",
            Category: Category.Feature,
            tagIds: new List<Guid>()
        );

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("Title field cannot be empty");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_TagIdsAreInvalid()
    {
        _mockTagRepo
            .Setup(r => r.GetByIdsAsync(
                It.IsAny<List<Guid>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Tag>());

        var command = new CreateLogCommand(
            UserId: Guid.NewGuid(),
            Title: "test title",
            Description: "test desc",
            Category: Category.Feature,
            tagIds: new List<Guid> {Guid.NewGuid()}
        );

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("One or more tags IDs are invalid");
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_allInputsAreValid()
    {

        var logId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        _mockTagRepo
            .Setup(r => r.GetByIdsAsync(
                It.IsAny<List<Guid>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Tag>());

        _mockLogRepo
            .Setup(r => r.CreateAsync(
                It.IsAny<Log>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync( new Log
            {
                Id = logId,
                Title = "test",
                Description = "test desc",
                Category = Category.Feature,
                LogTags = new List<LogTags>(),
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            });


            var command = new CreateLogCommand(
                UserId: userId,
                Title: "test",
                Description: "test desc",
                Category: Category.Feature,
                tagIds: new List<Guid>()
            );

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Title.Should().Be("test");
            result.Value.Description.Should().Be("test desc");
            result.Value.UserId.Should().Be(userId);

    }
}