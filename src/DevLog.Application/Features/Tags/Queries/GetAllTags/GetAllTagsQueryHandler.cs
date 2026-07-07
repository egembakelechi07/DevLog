using DevLog.Application.DTOs;
using DevLog.Domain.Entities;
using DevLog.Domain.Interfaces;
using DevLog.Shared;
using DevLog.Shared.Responses;
using MediatR;

namespace DevLog.Application.Features.Tags.Queries.GetAllTags;

public class GetAllTagsQueryHandler : IRequestHandler<GetAllTagsQuery, Result<PaginatedResponse<TagDto>>>
{
    private readonly ITagRepository _repository;

    public GetAllTagsQueryHandler(ITagRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<PaginatedResponse<TagDto>>> Handle(GetAllTagsQuery request , CancellationToken cancellationToken)
    {
        var errors = new List<string>();

        if (request.Page < 1)
        errors.Add("Page number must be greater than 0.");

        if (request.PageSize < 1 || request.PageSize > 50)
        errors.Add("Page size must be between 1 and 50.");

        if(errors.Any())
        return Result<PaginatedResponse<TagDto>>.Fail(errors);

        var (tags,totalcount) = await _repository.GetAllAsync(request.Page, request.PageSize, cancellationToken);

        var tagDtos = tags.Select(t => new TagDto
        {
            Id = t.Id,
            Name = t.Name,
            IsCustom = t.IsCustom,
            Description = t.Description,
            CreatedAt = t.CreatedAt,
            UpdatedAt = t.UpdatedAt 
        }).ToList();

        var response = new PaginatedResponse<TagDto>
        {
            Data = tagDtos,
            TotalCount = totalcount,
            Page = request.Page,
            PageSize = request.PageSize
        };

        return Result<PaginatedResponse<TagDto>>.Success(response);
    }
}