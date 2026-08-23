using DevLog.Application.DTOs;
using DevLog.Shared;
using MediatR;

namespace DevLog.Application.Features.Logs.Queries.GetSingleLog;

public record GetSingleLogQuery(Guid Id, Guid UserId) : IRequest<Result<LogDto>>;