using DevLog.Application.DTOs;
using DevLog.Application.Features.Users.Commands.RegisterUser;
using DevLog.Domain.Interfaces;
using DevLog.Domain.Entities;
using MediatR;
using DevLog.Shared;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result<UserDto>>
{
    private readonly IUserRepository _repository;
    public RegisterUserCommandHandler( IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<UserDto>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Name))
        errors.Add("Name field cannot be empty");
        
        var NormalizedEmail = request.Email.Trim().ToLower();

        if (string.IsNullOrWhiteSpace(NormalizedEmail))
        errors.Add("Email field cannot be empty");

        if (string.IsNullOrWhiteSpace(request.Password))
        errors.Add("Password field cannot be empty");

        if (string.IsNullOrWhiteSpace(request.ConfirmPassword))
        errors.Add("Confirm Password field cannot be empty");

        if (request.Password != request.ConfirmPassword)
        errors.Add("Password and Confirm Password do not match");

        if(!IsValidPassword(request.Password))
        errors.Add("Password must be at least 8 characters and contain at least one number and special character");

        var exists = await _repository.ExistsAsync(request.Email, cancellationToken);
        if (exists)
        errors.Add("User With this email already exists");

        if(errors.Any())
        return Result<UserDto>.Fail(errors);

        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            PasswordHash = request.Password
        };

        var created = await _repository.CreateAsync(user, cancellationToken);

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