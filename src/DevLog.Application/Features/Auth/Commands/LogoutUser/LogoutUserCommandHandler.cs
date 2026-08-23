using MediatR;
using DevLog.Application.Features.Auth.Commands.LogoutUser;
using DevLog.Shared;
using DevLog.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace DevLog.Application.Features.Auth.Commands.LogoutUser;

public class LogoutUserCommandHandler : IRequestHandler<LogoutUserCommand, Result<bool>>
{
    private readonly IUserRepository _repository;
    private readonly ILogger<LogoutUserCommandHandler> _logger;

    public LogoutUserCommandHandler(IUserRepository repository, ILogger<LogoutUserCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(LogoutUserCommand request, CancellationToken cancellationToken)
    {
        await _repository.LogoutAsync(request.userId, cancellationToken);

        _logger.LogInformation("Logout successful: {UserId}", request.userId);
        return Result<bool>.Success(true);
    }
}