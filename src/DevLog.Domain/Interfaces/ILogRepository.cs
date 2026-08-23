using DevLog.Domain.Entities;

namespace DevLog.Domain.Interfaces;


public interface ILogRepository
{
    Task<Log?> GetByIdAsync(Guid Id, Guid userId, CancellationToken cancellationToken);
    Task<(List<Log> logs, int totalCount)> GetAllLogsAsync( Guid UserId, int page, int pageSize, List<Guid>? TagIds = null, CancellationToken cancellationToken = default);
    Task<Log> CreateAsync(Log log, CancellationToken cancellationToken);
    Task<Log> UpdateAsync(Log log, CancellationToken cancellationToken);
    Task DeleteAsync(Log log, CancellationToken cancellationToken);
}