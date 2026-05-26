using DevLog.Application.DTOs;
using MediatR;

namespace DevLog.Application.Features.Users.Commands.RegisterUser;
public record RegisterUserCommand(string Name, string Email, string Password, string ConfirmPassword) : IRequest<UserDto>;