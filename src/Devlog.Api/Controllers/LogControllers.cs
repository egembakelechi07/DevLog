using System.Security.Claims;
using DevLog.Application.DTOs;
using DevLog.Application.Features.Logs.Commands.CreateLog;
using DevLog.Application.Features.Logs.Commands.DeleteLog;
using DevLog.Application.Features.Logs.Commands.UpdateLog;
using DevLog.Application.Features.Logs.Queries.GetAllLogs;
using DevLog.Application.Features.Logs.Queries.GetSingleLog;
using DevLog.Shared.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.IdentityModel.Tokens;


namespace DevLog.Api.Controllers;

[ApiController]
[Route("api/v1/logs")]

public class LogController : ControllerBase
{
    private readonly IMediator _mediator;

    public LogController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateLogAsync([FromBody] CreateLogDto dto, CancellationToken cancellationToken = default)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        
        if(userIdClaim == null)
        return Unauthorized(ApiResponse<LogDto>.Failure("User not authenticated", null));

        var userId = Guid.Parse(userIdClaim.Value);

        var result = await _mediator.Send( new CreateLogCommand(userId, dto.Title,
        dto.Description,dto.Category,dto.TagIds), cancellationToken);

        if(!result.IsSuccess)
        return BadRequest(ApiResponse<LogDto>.Failure("Failed to create log", result.Errors));

        return StatusCode(201, ApiResponse<LogDto>.Success(result.Value!, "Log Created Successfully"));


    }

    [HttpPut("{Id}")]
    public async Task<IActionResult> UpdateLogAsync(Guid logId, [FromBody] UpdateLogDto dto, CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if(userIdClaim == null)
        return Unauthorized(ApiResponse<LogDto>.Failure("User not authenticated", null));

        var userId = Guid.Parse(userIdClaim.Value);

        var result = await _mediator.Send(new UpdateLogCommand(logId, userId, dto.Title, dto.Description
        , dto.Category, dto.TagIds), cancellationToken);

        if(!result.IsSuccess)
        return BadRequest(ApiResponse<LogDto>.Failure("Failed to Update log", result.Errors));

        return Ok(ApiResponse<LogDto>.Success(result.Value!, "Log Updated Successfully"));



    }

    [HttpGet("{Id}")]
    public async Task<IActionResult> GetSingleLog(Guid Id, CancellationToken cancellationToken = default)
    {

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if(userIdClaim == null)
        return Unauthorized(ApiResponse<LogDto>.Failure("User not authenticated", null));

        var userId = Guid.Parse(userIdClaim.Value);


        var result = await _mediator.Send(new GetSingleLogQuery(Id, userId), cancellationToken);

        if(!result.IsSuccess)
        return NotFound(ApiResponse<LogDto>.Failure($"Log with ID {Id} was not found", result.Errors));

        return Ok(ApiResponse<LogDto>.Success(result.Value!, "Log Retrieved Successfully"));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllLogsAsync([FromQuery] int page = 1, [FromQuery] int pageSize = 50, [FromQuery] List<Guid>? TagIds = null, CancellationToken cancellationToken = default)
    {
        var UserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if(UserIdClaim == null)
        return Unauthorized(ApiResponse<LogDto>.Failure("User not Authenticated", null));

        var userId = Guid.Parse(UserIdClaim.Value);

        var result = await _mediator.Send(new GetAllLogsQuery(userId, page, pageSize), cancellationToken);
        if(!result.IsSuccess)
        return BadRequest(ApiResponse<PaginatedResponse<LogDto>>.Failure("Failed to retrieve logs", result.Errors));

        return Ok(ApiResponse<PaginatedResponse<LogDto>>.Success(result.Value!, "Logs retrieved successfully"));

    }

    [HttpDelete("{Id}")]
    public async Task<IActionResult> DeleteLogAsync(Guid Id, CancellationToken cancellationToken = default)
    {
        var UserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if(UserIdClaim == null)
        return Unauthorized(ApiResponse<LogDto>.Failure("User not Authenticated", null));

        var userId = Guid.Parse(UserIdClaim.Value);

        var result = await _mediator.Send(new DeleteLogCommand(userId, Id),cancellationToken);
        if(!result.IsSuccess)
        return NotFound(ApiResponse<PaginatedResponse<LogDto>>.Failure("Log not found", result.Errors));

        return NoContent();



    }
}
