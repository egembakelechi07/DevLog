using DevLog.Application.DTOs;
using DevLog.Domain.Interfaces;
using DevLog.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DevLog.Application.Features.Logs.Queries.GetSingleLog;

public class GetSingleLogQueryHandler : IRequestHandler<GetSingleLogQuery, Result<LogDto>>
{
    private readonly ILogRepository _repository;
    private readonly ILogger<GetSingleLogQueryHandler> _logger;

    public GetSingleLogQueryHandler(ILogRepository repository, ILogger<GetSingleLogQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<LogDto>> Handle(GetSingleLogQuery request, CancellationToken cancellationToken)
    {
        var log = await _repository.GetByIdAsync(request.Id, request.UserId, cancellationToken);

        if(log == null)
        {
            _logger.LogWarning("Log not found : {LogId} for {UserId}", request.Id, request.UserId);
            return Result<LogDto>.Fail($" log with this Id {request.Id} not found");
        }

        return Result<LogDto>.Success( new LogDto
        {
            Id = log.Id,
            Title = log.Title,
            Description = log.Description,
            Category = log.Category.ToString(),
            UserId = log.UserId,
            Tags = log.LogTags.Select(lt => new TagDto
            {
                Id = lt.Tag.Id,
                Name = lt.Tag.Name,
                Description = lt.Tag.Description,
                IsCustom = lt.Tag.IsCustom,
                CreatedAt = lt.Tag.CreatedAt,
                UpdatedAt = lt.Tag.UpdatedAt
            }).ToList(),
            CreatedAt = log.CreatedAt,
            LastUpdatedAt = log.LastUpdatedAt
        });
    }
}