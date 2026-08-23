using DevLog.Application.DTOs;
using DevLog.Shared;
using MediatR;

namespace DevLog.Application.Features.Logs.Commands.DeleteLog;


public record DeleteLogCommand(Guid UserId, Guid LogId) : IRequest<Result<bool>>;