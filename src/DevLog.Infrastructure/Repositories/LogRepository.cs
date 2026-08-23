using DevLog.Infrastructure.Data;
using DevLog.Domain.Interfaces;
using DevLog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DevLog.Infrastructure.Repositories;

public class LogRepository : ILogRepository
{
    private readonly ApplicationDbContext _context;

    public LogRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Log?> GetByIdAsync(Guid Id, Guid userId, CancellationToken cancellationToken)
    {
        return await _context.Logs
        .Include(l => l.LogTags)
            .ThenInclude(lt => lt.Tag)
        .FirstOrDefaultAsync(l => l.Id == Id && l.UserId == userId, cancellationToken);
    }

    public async Task<(List<Log> logs, int totalCount)> GetAllLogsAsync( Guid UserId, int page, int pageSize, List<Guid>? TagIds , CancellationToken cancellationToken = default)
    {

        var query = _context.Logs
        .Where(l => l.UserId == UserId)
        .Include(l => l.LogTags)
            .ThenInclude(lt => lt.Tag)
            .AsQueryable();

        if(TagIds != null && TagIds.Any())
        {
            query = query.Where(l => l.LogTags.Any(lt => TagIds.Contains(lt.TagId)));
        }
        var totalCount = await query.CountAsync(cancellationToken);

        var logs = await query
        .OrderByDescending(l => l.CreatedAt)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync(cancellationToken);

        return (logs, totalCount) ;
    }

    public async Task<Log> CreateAsync(Log log, CancellationToken cancellationToken)
    {
        await _context.Logs.AddAsync(log, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return log;
    }

    public async Task<Log> UpdateAsync(Log log, CancellationToken cancellationToken)
    {
        log.LastUpdatedAt = DateTime.UtcNow;
        _context.Logs.Update(log);
        await _context.SaveChangesAsync(cancellationToken);
        return log;
    }

    public async Task DeleteAsync(Log log, CancellationToken cancellationToken)
    {
        _context.Logs.Remove(log);
        await _context.SaveChangesAsync(cancellationToken);
    }
}