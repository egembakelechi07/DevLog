using MediatR;
using Microsoft.AspNetCore.Mvc;
using DevLog.Application.Features.Auth.Commands.RegisterUser;
using DevLog.Application.Features.Auth.Commands.LoginUser;
using DevLog.Shared.Responses;
using DevLog.Application.DTOs;
using DevLog.Application.Features.Auth.Commands.LogoutUser;
using DevLog.Application.Features.Auth.Queries.GetUserInfo;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Devlog.Api.Controllers;


[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // POST /api/v1/Auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(command, cancellationToken);

        if(!result.IsSuccess)
        return BadRequest(ApiResponse<UserDto>.Failure("Validation Failed", result.Errors));

        return StatusCode(201, ApiResponse<UserDto>.Success(result.Value! ,"User Registeration Succesfully"));
    }

    // POST /api/v1/Auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserCommand command, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return Unauthorized(ApiResponse<LoginDto>.Failure(
                "Login failed.",
                result.Errors));

        return Ok(ApiResponse<LoginDto>.Success(result.Value!, "Login successful."));
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken = default)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if(userIdClaim == null)
        return Unauthorized(ApiResponse<bool>.Failure("User not Authenticated", null));

        var userId = Guid.Parse(userIdClaim.Value);
        await _mediator.Send(new LogoutUserCommand(userId), cancellationToken);
        return Ok(ApiResponse<bool>.Success(true,"Logged out succesfully."));
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetUserInfo(CancellationToken cancellationToken = default)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if(userIdClaim == null)
        return Unauthorized(ApiResponse<UserDto>.Failure("User Not Authenticated", null));

        var userId = Guid.Parse(userIdClaim.Value);
        var result = await _mediator.Send(new GetUserInfoQuery(userId), cancellationToken);

        if(!result.IsSuccess)
        return NotFound(ApiResponse<UserDto>.Failure("Retrieving User Info Failed", result.Errors));

        return Ok(ApiResponse<UserDto>.Success(result.Value!, " User Information Retrieved Successfully"));
    }
}