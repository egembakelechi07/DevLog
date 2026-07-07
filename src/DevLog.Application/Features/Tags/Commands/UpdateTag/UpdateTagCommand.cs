using DevLog.Application.DTOs;
using DevLog.Shared;
using MediatR;

namespace DevLog.Application.Features.Tags.Commands.UpdateTag;

public record UpdateTagCommand(Guid Id, string Name, string Description) : IRequest<Result<TagDto>>;