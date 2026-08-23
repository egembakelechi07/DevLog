using DevLog.Application.DTOs;
using DevLog.Domain.Interfaces;
using DevLog.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DevLog.Application.Features.Auth.Queries.GetUserInfo;

public class GetUserInfoQueryHandler : IRequestHandler<GetUserInfoQuery, Result<UserDto>>
{
    private readonly IUserRepository _repository;
    private readonly ILogger<GetUserInfoQueryHandler> _logger;

    public GetUserInfoQueryHandler(IUserRepository repository, ILogger<GetUserInfoQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<UserDto>> Handle(GetUserInfoQuery request, CancellationToken cancellationToken)
    {
        var user = await _repository.GetByIdAsync(request.UserId, cancellationToken);
        if(user == null)
        {
            _logger.LogWarning("User not found for valid JWT claim : {UserId}", request.UserId);
            return Result<UserDto>. Fail($"user with the Id {request.UserId} not found");
   
        }
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