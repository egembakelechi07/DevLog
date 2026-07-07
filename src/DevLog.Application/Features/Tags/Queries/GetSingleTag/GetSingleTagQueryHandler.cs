using DevLog.Application.DTOs;
using DevLog.Application.Features.Tags.Queries.GetSingleTag;
using DevLog.Domain.Entities;
using DevLog.Domain.Interfaces;
using DevLog.Shared;
using MediatR;

namespace DevLog.Application.Features.Tags.Queries.GetSingleTag;

public class GetSingleTagQueryHandler : IRequestHandler<GetSingleTagQuery, Result<TagDto>>
{
    private readonly ITagRepository _repository;

    public GetSingleTagQueryHandler(ITagRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<TagDto>> Handle(GetSingleTagQuery request, CancellationToken cancellationToken)
    {
        var tag = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if(tag == null)
        return Result<TagDto>.Fail($"Tag with Id {request.Id} not found");

        return Result<TagDto>.Success( new TagDto
        {
            Id = tag.Id,
            Name = tag.Name,
            Description = tag.Description,
            IsCustom = tag.IsCustom,
            CreatedAt = tag.CreatedAt,
            UpdatedAt = tag.UpdatedAt
        });

    }
}