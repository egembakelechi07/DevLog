using DevLog.Application.DTOs;
using MediatR;
using DevLog.Shared;

namespace DevLog.Application.Features.Tags.Commands.CreateTag;

public record CreateTagCommand(string Name, string Description) : IRequest<Result<TagDto>>;