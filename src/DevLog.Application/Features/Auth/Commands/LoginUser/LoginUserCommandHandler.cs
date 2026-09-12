using DevLog.Application.DTOs;
using DevLog.Domain.Entities;
using DevLog.Domain.Interfaces;
using DevLog.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DevLog.Application.Features.Auth.Commands.LoginUser;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, Result<LoginDto>>
{
    private readonly IJwtService _jwtService;
    private readonly IUserRepository _repository;
    private readonly ILogger<LoginUserCommandHandler> _logger;

    public LoginUserCommandHandler(IJwtService jwtService, IUserRepository repository, ILogger<LoginUserCommandHandler> logger)
    {
        _jwtService = jwtService;
        _repository = repository;
        _logger = logger ;
    }

    public async Task<Result<LoginDto>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var errors = new List<string>();

        if(string.IsNullOrWhiteSpace(request.email))
        errors.Add("Email Field Cannot be Empty");
        if(string.IsNullOrWhiteSpace(request.password))
        errors.Add("Password Field cannot be empty");

        if (errors.Any())
        {
            _logger.LogWarning("Login failed - validation errors for {Email} : {Errors}",
             request.email, 
             string.Join("," , errors));

            return Result<LoginDto>.Fail(errors);
        }

        var normalizedEmail = request.email.Trim().ToLower();

        var user = await _repository.GetByEmailAsync(normalizedEmail, cancellationToken);

        if(user == null)
        {
            _logger.LogWarning("Login failed - user not found: {Email}", request.email);
            return Result<LoginDto>.Fail("Invalid Email or Password");
        }
        

        bool isValidPassword = _repository.IsValidPassword(request.password, user.PasswordHash);
        if(isValidPassword == false)
        {
            _logger.LogWarning("login Failed - Invalid password for user :{Email}", request.email);
            return Result<LoginDto>.Fail("Invalid Email or password");
        }

        //Generate access Token
        var token = _jwtService.GenerateToken(user.Id, user.Name, user.Email);
        var refreshtoken = _jwtService.GenerateRefreshToken();
        var refreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);

        await _repository.UpdateRefreshTokenAsync(user.Id,refreshtoken, refreshTokenExpiresAt);

        _logger.LogInformation("Login successful : {Email}", request.email);

        return Result<LoginDto>.Success(new LoginDto
        {
            AccessToken = token,
            TokenType = "Bearer",
            ExpiresIn = 3600,
            RefreshToken = null,
            User = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                LastUpdatedAt = user.LastUpdatedAt,
            }
        });
    }
}