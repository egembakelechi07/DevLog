using DevLog.Application.DTOs;
using DevLog.Application.Features.Auth.Commands.RegisterUser;
using DevLog.Domain.Interfaces;
using DevLog.Domain.Entities;
using MediatR;
using DevLog.Shared;
using Microsoft.Extensions.Logging;

namespace DevLog.Application.Features.Auth.Commands.RegisterUser;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result<UserDto>>
{
    private readonly IUserRepository _repository;
    private readonly ILogger<RegisterUserCommandHandler> _logger;
    public RegisterUserCommandHandler( IUserRepository repository, ILogger<RegisterUserCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<UserDto>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Name))
        errors.Add("Name field cannot be empty");

        if (string.IsNullOrWhiteSpace(request.Email))
        errors.Add("Email field cannot be empty");

        if (string.IsNullOrWhiteSpace(request.Password))
        errors.Add("Password field cannot be empty");

        if (string.IsNullOrWhiteSpace(request.ConfirmPassword))
        errors.Add("Confirm Password field cannot be empty");

        if (request.Password != request.ConfirmPassword)
        errors.Add("Password and Confirm Password do not match");

        if(!IsValidPassword(request.Password))
        errors.Add("Password must be at least 8 characters and contain at least one number and special character");

        if (errors.Any())
        {
           _logger.LogWarning("Registration Failed - validation errors for {Email}: {Errors}", 
           request.Email
           ,string.Join(", ", errors));

           return Result<UserDto>.Fail(errors);
        }

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var exists = await _repository.ExistsAsync(normalizedEmail, cancellationToken);
        if (exists)
        {
            _logger.LogWarning("Registration failed - email already exists: {Email}"
            , normalizedEmail);

            return Result<UserDto>. Fail("Email already exists");
        }

        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            PasswordHash = request.Password
        };

        var created = await _repository.CreateAsync(user, cancellationToken);

        _logger.LogInformation("Registration successful : {UserId} {Email}", created.Id, created.Email);

        return Result<UserDto>.Success(new UserDto
        {
            Id = created.Id,
            Name = created.Name,
            Email = created.Email,
            CreatedAt = created.CreatedAt,
        }); 
    }

    private static bool IsValidPassword(string password)
    {
        if(password.Length < 8 )
        return false;

        bool hasUppercase = password.Any(char.IsUpper);
        bool hasLowercase = password.Any(char.IsLower);
        bool hasDigit = password.Any(char.IsDigit);
        bool hasSpecialChar = password.Any(ch => !char.IsLetterOrDigit(ch));

        return hasUppercase && hasLowercase && hasDigit && hasSpecialChar;

    }
}