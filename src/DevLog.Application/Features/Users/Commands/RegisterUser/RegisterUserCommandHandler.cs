using DevLog.Application.DTOs;
using DevLog.Application.Features.Users.Commands.RegisterUser;
using DevLog.Domain.Interfaces;
using DevLog.Domain.Entities;
using MediatR;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, UserDto>
{
    private readonly IUserRepository _repository;
    public RegisterUserCommandHandler( IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<UserDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var exists = await _repository.ExistsAsync(request.Email);
        
        if(exists) 
        throw new InvalidOperationException("A User With Email Address already exists.");

        if(string.IsNullOrWhiteSpace(request.Name))
        throw new InvalidOperationException("Name Field Cannot be Empty");
        if(string.IsNullOrWhiteSpace(request.Email))
        throw new InvalidOperationException("Email Field Cannot be Empty");
        if(string.IsNullOrWhiteSpace(request.Password))
        throw new InvalidOperationException("Password Field Cannot be Empty");
        if(string.IsNullOrWhiteSpace(request.ConfirmPassword))
        throw new InvalidOperationException("ConfirmPassword Field Cannot be Empty");

        if(request.Password != request.ConfirmPassword)
        throw new InvalidOperationException("Password and ConfirmPassword don't match");

                // Hash the password before storing
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            PasswordHash = passwordHash
        };

        var created = await _repository.CreateAsync(user);

        return new UserDto
        {
            Id = created.Id,
            Name = created.Name,
            Email = created.Email,
            CreatedAt = created.CreatedAt,
            LastUpdatedAt = created.LastUpdatedAt
        };
    }
}