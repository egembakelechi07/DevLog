using DevLog.Application.DTOs;
using DevLog.Domain.Entities;
using DevLog.Domain.Interfaces;
using DevLog.Shared;
using MediatR;

namespace DevLog.Application.Features.Users.Queries.GetUserInfo;

public class GetUserInfoQueryHandler : IRequestHandler<GetUserInfoQuery, Result<UserDto>>
{
    private readonly IUserRepository _repository;

    public GetUserInfoQueryHandler(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<UserDto>> Handle(GetUserInfoQuery request, CancellationToken cancellationToken)
    {
        var user = await _repository.GetByIdAsync(request.UserId, cancellationToken);
        if(user == null)
        return Result<UserDto>. Fail($"user with the Id {request.UserId} not found");

        return Result<UserDto>.Success(new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            IsDisabled = user.IsDisabled,
            CreatedAt = user.CreatedAt,
            LastUpdatedAt = user.LastUpdatedAt
        });
    }
}