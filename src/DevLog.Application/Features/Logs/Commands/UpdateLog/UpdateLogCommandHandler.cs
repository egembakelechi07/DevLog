using DevLog.Application.DTOs;
using DevLog.Domain.Entities;
using DevLog.Domain.Interfaces;
using DevLog.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DevLog.Application.Features.Logs.Commands.UpdateLog;

public class UpdateLogCommandHandler : IRequestHandler<UpdateLogCommand, Result<LogDto>>
{
    private readonly ILogRepository _repository;
    private readonly ITagRepository _tagRepository;
    private readonly ILogger<UpdateLogCommandHandler> _logger;

    public UpdateLogCommandHandler(ILogRepository repository, ITagRepository tagRepository, ILogger<UpdateLogCommandHandler> logger)
    {
        _repository = repository;
        _tagRepository = tagRepository;
        _logger = logger;
    }

    public async Task<Result<LogDto>> Handle(UpdateLogCommand request, CancellationToken cancellationToken)
    {

        var log = await _repository.GetByIdAsync(request.logID, request.UserId, cancellationToken);
        if(log == null)
        {
            _logger.LogWarning("Log not found for user : {LogId} for {UserId}", request.logID, request.UserId);
            return Result<LogDto>.Fail($"log with ID {request.logID} not found");
        }
        
        var errors = new List<string>();

        if(string.IsNullOrWhiteSpace(request.Title))
        errors.Add("Title Field cannot be empty");
        if(string.IsNullOrWhiteSpace(request.Description))
        errors.Add("Description Field cannot be empty");

        if(!Enum.IsDefined(typeof(Category), request.Category))
        errors.Add("Invalid Category");

        if (errors.Any())
        {
            _logger.LogWarning("Updating log failed - validation errors : {Errors}", string.Join(",", errors));
            return Result<LogDto>.Fail(errors);
        }

        var tags = new List<Tag>();
        if (request.tagIds.Any())
        {
            tags = await _tagRepository.GetByIdsAsync(request.tagIds,cancellationToken);
            if(tags.Count != request.tagIds.Count)
            {
                _logger.LogWarning("Fetching TagIds Failed : {RequestedCount}requestedCount, {Found}found, {TagIds}", request.tagIds.Count, tags.Count, request.tagIds);
                return Result<LogDto>.Fail("One or more tags are invalid");
            }
        }

        log.Title = request.Title;
        log.Description = request.Description;
        log.Category = request.Category;

        log.LogTags.Clear();
        foreach(var tag in tags)
        {
            log.LogTags.Add(new LogTags
            {
                LogId = log.Id,
                TagId = tag.Id
            });
        }

        var updated = await _repository.UpdateAsync(log, cancellationToken);
        _logger.LogInformation("Log updated succesfully : {LogId} for user {UserId}", request.logID, request.UserId);

         return Result<LogDto>.Success(new LogDto
        {
            Id = updated.Id,
            Title = updated.Title,
            Description = updated.Description,
            Category = updated.Category.ToString(),
            UserId = updated.UserId,
            Tags = tags.Select(t => new TagDto
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                IsCustom = t.IsCustom,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt
            }).ToList(),
            CreatedAt = updated.CreatedAt,
            LastUpdatedAt = updated.LastUpdatedAt
            });
    }
}