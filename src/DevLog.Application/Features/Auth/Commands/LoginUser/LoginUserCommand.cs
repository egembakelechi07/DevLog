using DevLog.Application.DTOs;
using DevLog.Shared;
using MediatR;

namespace DevLog.Application.Features.Auth.Commands.LoginUser;

public record LoginUserCommand(string email, string password, bool rememberMe = false) : IRequest<Result<LoginDto>>;