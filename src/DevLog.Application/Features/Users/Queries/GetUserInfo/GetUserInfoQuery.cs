using DevLog.Application.DTOs;
using DevLog.Shared;
using MediatR;

public record GetUserInfoQuery(Guid UserId) : IRequest<Result<UserDto>>;