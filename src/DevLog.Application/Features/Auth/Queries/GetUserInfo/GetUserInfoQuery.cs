using DevLog.Application.DTOs;
using DevLog.Shared;
using MediatR;

namespace DevLog.Application.Features.Auth.Queries.GetUserInfo;
public record GetUserInfoQuery(Guid UserId) : IRequest<Result<UserDto>>;