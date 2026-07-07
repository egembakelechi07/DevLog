using DevLog.Application.DTOs;
using MediatR;
using DevLog.Domain.Interfaces;
using DevLog.Shared;
using DevLog.Domain.Entities;
using DevLog.Application.Features.Tags.Commands.CreateTag;

namespace DevLog.Application.Features.Tags.Commands.CreateTag;

public class CreateTagCommandHandler : IRequestHandler<CreateTagCommand, Result<TagDto>>
{
    private readonly ITagRepository _repository;

    public CreateTagCommandHandler(ITagRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<TagDto>> Handle(CreateTagCommand request, CancellationToken cancellationToken)
    {
        var errors = new List<string>();

        if(string.IsNullOrWhiteSpace(request.Name))
        errors.Add("Name field cannot be empty");
        if(string.IsNullOrWhiteSpace(request.Description))
        errors.Add("Description field cannot be empty");

        if(errors.Any())
        return Result<TagDto>.Fail(errors);

        var normalizedName = request.Name.Trim();
        var exists = await _repository.ExistsAsync(normalizedName, cancellationToken);

        if(exists)
        return Result<TagDto>.Fail("This tag already exists");

        var tag = new Tag
        {
            Name = normalizedName,
            Description = request.Description,
            IsCustom = true,
            CreatedAt = DateTime.UtcNow
        };
        
        var created = await _repository.CreateAsync(tag, cancellationToken);

        return Result<TagDto>.Success(new TagDto
        {
            Id = created.Id,
            Name = created.Name,
            Description = created.Description,
            IsCustom = created.IsCustom,
            CreatedAt = created.CreatedAt
        });
    }
}