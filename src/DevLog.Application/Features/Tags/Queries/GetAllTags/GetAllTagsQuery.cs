using DevLog.Application.DTOs;
using DevLog.Shared;
using DevLog.Shared.Responses;
using MediatR;

namespace DevLog.Application.Features.Tags.Queries.GetAllTags;

public record GetAllTagsQuery(int Page = 1, int PageSize = 50) : IRequest<Result<PaginatedResponse<TagDto>>>;