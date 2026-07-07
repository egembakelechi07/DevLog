using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using DevLog.Application.DTOs;
using DevLog.Application.Features.Tags.Commands.UpdateTag;
using DevLog.Domain.Entities;
using DevLog.Domain.Interfaces;
using DevLog.Shared;
using MediatR;

namespace DevLog.Application.Features.Tags.Commands.UpdateTag;

public class UpdateTagCommandHandler : IRequestHandler<UpdateTagCommand, Result<TagDto>>
{
    private readonly ITagRepository _repository;

    public UpdateTagCommandHandler(ITagRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<TagDto>> Handle(UpdateTagCommand request, CancellationToken cancellationToken)
    {
        var tag = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if(tag == null)
        return Result<TagDto>.Fail($"Tag with ID {request.Id} was not found");

        if(!tag.IsCustom)
        return Result<TagDto>.Fail("Predefined Tags cannot be updated");

        if(request.Name != tag.Name)
        {
            var exists = await  _repository.ExistsAsync(request.Name, cancellationToken);
            if(exists)
            return Result<TagDto>.Fail("A Tag with this name already exiasts");
        }

        tag.Name = request.Name;
        tag.Description = request.Description;

        var updated = await _repository.UpdateAsync(tag, cancellationToken);

        return Result<TagDto>.Success( new TagDto
        {
            Id =  updated.Id,
            Name = updated.Name,
            Description = updated.Description,
            IsCustom = updated.IsCustom,
            CreatedAt = updated.CreatedAt,
            UpdatedAt = updated.UpdatedAt

        });

        
        



    }
}