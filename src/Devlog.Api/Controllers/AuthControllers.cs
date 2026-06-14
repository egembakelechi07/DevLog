using MediatR;
using Microsoft.AspNetCore.Mvc;
using DevLog.Application.Features.Users.Commands.RegisterUser;
using DevLog.Application.Features.Users.Commands.LoginUser;
using DevLog.Shared.Responses;
using DevLog.Application.DTOs;
using DevLog.Application.Features.Users.Commands.LogoutUser;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace Devlog.Api.Controllers;

[ApiController]
[Route("api/v1/Auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // POST /api/v1/Auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
    {
        var result = await _mediator.Send(command);

        if(!result.IsSuccess)
        return BadRequest(ApiResponse<UserDto>.Failure("Validation Failed", result.Errors));

        return StatusCode(201, ApiResponse<UserDto>.Success(result.Value! ,"User Registeration Succesfully"));
    }

    // POST /api/v1/Auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return Unauthorized(ApiResponse<LoginDto>.Failure(
                "Login failed.",
                result.Errors));

        return Ok(ApiResponse<LoginDto>.Success(result.Value!, "Login successful."));
    }

    [HttpPost("Logout")]
    public async Task<IActionResult> Logout()
    {
        var userIdClaim = User.FindFirst("sub");

        if(userIdClaim == null)
        return Unauthorized(ApiResponse<bool>.Failure("User not Authenticated", null));

        var userId = Guid.Parse(userIdClaim.Value);
        await _mediator.Send(new LogoutUserCommand(userId));
        return Ok(ApiResponse<bool>.Success(true,"Logged out succesfully."));
    }
}