using DevLog.Application.DTOs;
using DevLog.Application.Features.Tags.Commands.CreateTag;
using DevLog.Application.Features.Tags.Commands.UpdateTag;
using DevLog.Application.Features.Tags.Queries.GetAllTags;
using DevLog.Application.Features.Tags.Queries.GetSingleTag;
using DevLog.Shared.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Devlog.Api.Controllers;

[ApiController]
[Route("api/v1/tags")]
public class TagController : ControllerBase
{
    private readonly IMediator _mediator;

    public TagController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateTag([FromBody] CreateTagCommand command, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(command, cancellationToken);

        if(!result.IsSuccess)
        return BadRequest(ApiResponse<TagDto>.Failure("Failed to create tag", result.Errors));

        return StatusCode(201, ApiResponse<TagDto>.Success(result.Value!,"Tag Created Successfully"));
    }

    [HttpPut]
    public async Task<IActionResult> UpdateTag( Guid Id, [FromBody] UpdateTagDto dto, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send( new UpdateTagCommand(Id, dto.Name, dto.Description), cancellationToken);
        if(!result.IsSuccess)
        return NotFound(ApiResponse<TagDto>.Failure($"Failed to update tag with Id {Id}" , result.Errors));

        return Ok(ApiResponse<TagDto>.Success(result.Value!, "Tag updated Successfully"));
    }

    [HttpGet ("{Id}")]
    public async Task<IActionResult> GetSingleTag(Guid Id, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send( new GetSingleTagQuery(Id), cancellationToken);
        
        if(!result.IsSuccess)
        return NotFound(ApiResponse<TagDto>.Failure($"Tag with Id {Id} not found ", result.Errors));

        return Ok(ApiResponse<TagDto>.Success(result.Value!, "Tag Retrieved Successfully"));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int PageSize = 50, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetAllTagsQuery(page, PageSize), cancellationToken);

        if(!result.IsSuccess)
        return BadRequest(ApiResponse<PaginatedResponse<TagDto>>.Failure("Failed to retrieve tags", result.Errors));

        return Ok(ApiResponse<PaginatedResponse<TagDto>>.Success(result.Value!, "Tags retrieved Successfully."));

    }


}