using MediatR;
using DevLog.Application.Features.Users.Commands.LogoutUser;
using DevLog.Shared;
using DevLog.Domain.Interfaces;

namespace DevLog.Application.Features.Users.Commands.LogoutUser;

public class LogoutUserCommandHandler : IRequestHandler<LogoutUserCommand, Result<bool>>
{
    private readonly IUserRepository _repository;

    public LogoutUserCommandHandler(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<bool>> Handle(LogoutUserCommand request, CancellationToken cancellationToken)
    {
        await _repository.LogoutAsync(request.userId);
        return Result<bool>.Success(true);
    }
}