using DevLog.Application.DTOs;
using DevLog.Domain.Entities;
using DevLog.Shared;
using MediatR;

namespace DevLog.Application.Features.Logs.Commands.UpdateLog;

public record UpdateLogCommand(Guid logID, Guid UserId, string Title,
string Description, Category Category, List<Guid> tagIds) : IRequest<Result<LogDto>>;