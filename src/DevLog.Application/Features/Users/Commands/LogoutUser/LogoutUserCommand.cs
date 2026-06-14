using DevLog.Shared;
using MediatR;

namespace DevLog.Application.Features.Users.Commands.LogoutUser;

public record LogoutUserCommand(Guid userId) : IRequest<Result<bool>>;