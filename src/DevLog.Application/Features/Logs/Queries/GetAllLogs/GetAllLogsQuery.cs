using DevLog.Shared;
using DevLog.Shared.Responses;
using DevLog.Application.DTOs;
using MediatR;

namespace DevLog.Application.Features.Logs.Queries.GetAllLogs;

public record GetAllLogsQuery(Guid UserId, int Page = 1, int PageSize = 50, List<Guid>? TagIds = null) : IRequest<Result<PaginatedResponse<LogDto>>>;