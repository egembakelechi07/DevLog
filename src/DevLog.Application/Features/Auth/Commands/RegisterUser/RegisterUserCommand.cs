using DevLog.Application.DTOs;
using DevLog.Shared;
using MediatR;

namespace DevLog.Application.Features.Auth.Commands.RegisterUser;
public record RegisterUserCommand(string Name, string Email, string Password, string ConfirmPassword) : IRequest<Result<UserDto>>;