using DevLog.Application.DTOs;
using DevLog.Domain.Entities;
using DevLog.Domain.Interfaces;
using DevLog.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DevLog.Application.Features.Logs.Commands.CreateLog;

public class CreateLogCommandHandler : IRequestHandler<CreateLogCommand, Result<LogDto>>
{
    private readonly ILogRepository _repository;
    private readonly ITagRepository _tagRepository;
    private readonly ILogger<CreateLogCommandHandler> _logger;
    public CreateLogCommandHandler(ILogRepository repository, ITagRepository tagRepository, ILogger<CreateLogCommandHandler> logger)
    {
        _repository = repository;
        _tagRepository = tagRepository;
        _logger = logger;
    }

    public async Task<Result<LogDto>> Handle(CreateLogCommand request, CancellationToken cancellationToken)
    {
        var errors = new List<string>();

        if(string.IsNullOrWhiteSpace(request.Title))
        errors.Add("Title field cannot be empty");

        if(string.IsNullOrWhiteSpace(request.Description))
        errors.Add("Description field cannot be empty");

        if(!Enum.IsDefined(typeof(Category), request.Category))
        errors.Add("Invalid Category");

        if (errors.Any())
        {
            _logger.LogWarning("Creating log Failed - validation errors : {Errors}",
            string.Join(",", errors));

            return Result<LogDto>.Fail(errors);
        }

        var tags = new List<Tag>();
        if (request.tagIds.Any())
        {
            tags = await _tagRepository.GetByIdsAsync(request.tagIds, cancellationToken);

            if(tags.Count() != request.tagIds.Count())
            {
                _logger.LogWarning("Tag validation failed - {RequestedCount}requested, {Found}found : {TagIds}", request.tagIds.Count , tags.Count , request.tagIds);
                return Result<LogDto>.Fail("One or more tags IDs are invalid");
            }
        }

        var logId = Guid.NewGuid();
        var log = new Log
        {
            Id = logId,
            Title = request.Title,
            Description = request.Description,
            Category = request.Category,
            LogTags = tags.Select(t => new LogTags
            {
                LogId = logId,
                TagId = t.Id
            }).ToList(),
            UserId = request.UserId,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _repository.CreateAsync(log, cancellationToken);
        _logger.LogInformation("Log Created successfully: {LogId} for user {UserId}", created.Id , created.UserId);

        return Result<LogDto>.Success(new LogDto
        {
            Id = created.Id,
            Title = created.Title,
            Description = created.Description,
            Category = created.Category.ToString(),
            UserId = created.UserId,
            Tags = tags.Select(t => new TagDto
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                IsCustom = t.IsCustom,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt
            }).ToList(),
            CreatedAt = created.CreatedAt
            });


    }
}