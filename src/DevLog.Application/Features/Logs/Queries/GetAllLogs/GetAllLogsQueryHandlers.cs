using System.Numerics;
using DevLog.Application.DTOs;
using DevLog.Domain.Interfaces;
using DevLog.Shared;
using DevLog.Shared.Responses;
using DevLog.Application.Features.Logs.Queries.GetAllLogs;
using MediatR;

namespace DevLog.Application.Features.Logs.Queries.GetAllLogs;

public class GetAllLogsQueryHandlers : IRequestHandler<GetAllLogsQuery, Result<PaginatedResponse<LogDto>>>
{
    private readonly ILogRepository _repository;

    public GetAllLogsQueryHandlers(ILogRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<PaginatedResponse<LogDto>>> Handle(GetAllLogsQuery request, CancellationToken cancellationToken)
    {
        if(request.Page < 1 || request.PageSize < 1 || request.PageSize > 50 )
        return Result<PaginatedResponse<LogDto>>.Fail("Invalid Pagination Parameters");

        var (logs,totalCount) = await _repository.GetAllLogsAsync(request.UserId, request.Page, request.PageSize, request.TagIds, cancellationToken);

        var logDtos = logs.Select(l => new LogDto
        {
           Id = l.Id,
           Title = l.Title,
           Description = l.Description,
           Category = l.Category.ToString(),
           UserId = l.UserId,
           Tags = l.LogTags.Select(lt => new TagDto
           {
               Id = lt.Tag.Id,
               Name = lt.Tag.Name,
               Description = lt.Tag.Description,
               IsCustom = lt.Tag.IsCustom,
               CreatedAt = lt.Tag.CreatedAt,
               UpdatedAt = lt.Tag.UpdatedAt
           }).ToList(),
           CreatedAt = l.CreatedAt,
           LastUpdatedAt = l.LastUpdatedAt
        }).ToList();

        var response =  new PaginatedResponse<LogDto>
        {
            Data = logDtos,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };

        return Result<PaginatedResponse<LogDto>>.Success(response);
    }
}