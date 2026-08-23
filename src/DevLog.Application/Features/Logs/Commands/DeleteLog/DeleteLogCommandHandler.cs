using DevLog.Domain.Interfaces;
using DevLog.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DevLog.Application.Features.Logs.Commands.DeleteLog;

public class DeleteLogCommandHandler : IRequestHandler<DeleteLogCommand, Result<bool>>
{
    private readonly ILogRepository _repository;
    private readonly ILogger<DeleteLogCommandHandler> _logger;

    public DeleteLogCommandHandler(ILogRepository repository, ILogger<DeleteLogCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(DeleteLogCommand request, CancellationToken cancellationToken)
    {
        var log = await _repository.GetByIdAsync(request.LogId, request.UserId, cancellationToken);

        if(log == null)
        {
            _logger.LogWarning("Log not found : {LogId} for user {UserId} ", request.LogId, request.UserId);
            return Result<bool>.Fail($"Log with ID {request.LogId} not found.");
            
        }
        await _repository.DeleteAsync(log,cancellationToken);

        _logger.LogInformation("Log deleted Succesfully for user : {UserId}", request.UserId);
        return Result<bool>.Success(true);
    }

}