using DevLog.Application.DTOs;
using DevLog.Shared;
using MediatR;

namespace DevLog.Application.Features.Tags.Queries.GetSingleTag;

public record GetSingleTagQuery(Guid Id) : IRequest<Result<TagDto>>;