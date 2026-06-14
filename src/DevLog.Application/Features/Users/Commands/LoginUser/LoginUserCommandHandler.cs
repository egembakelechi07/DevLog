using DevLog.Application.DTOs;
using DevLog.Domain.Interfaces;
using DevLog.Shared;
using MediatR;

namespace DevLog.Application.Features.Users.Commands.LoginUser;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, Result<LoginDto>>
{
    private readonly IJwtService _jwtService;
    private readonly IUserRepository _repository;

    public LoginUserCommandHandler(IJwtService jwtService, IUserRepository repository)
    {
        _jwtService = jwtService;
        _repository = repository;
    }

    public async Task<Result<LoginDto>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var errors = new List<string>();

        if(string.IsNullOrWhiteSpace(request.email))
        errors.Add("Email Field Cannot be Empty");
        if(string.IsNullOrWhiteSpace(request.password))
        errors.Add("Password Field cannot be empty");

        if(errors.Any())
        return Result<LoginDto>.Fail(errors);

        var normalizedEmail = request.email.Trim().ToLower();

        var user = await _repository.GetByEmailAsync(normalizedEmail);

        if(user == null)
        return Result<LoginDto>.Fail("Invalid Email or Passowrd");

        bool isValidPassword = _repository.IsValidPassword(request.password, user.PasswordHash);

        //Generate access Token
        var token = _jwtService.GenerateToken(user.Id, user.Name, user.Email);
        var refreshtoken = _jwtService.GenerateRefreshToken();
        var refreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);

        await _repository.UpdateRefreshTokenAsync(user.Id,refreshtoken, refreshTokenExpiresAt);

        return Result<LoginDto>.Success(new LoginDto
        {
            AccessToken = token,
            TokenType = "Bearer",
            ExpiresIn = 3600,
            RefreshToken = refreshtoken,
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