using DevLog.Domain.Interfaces;
using DevLog.Infrastructure.Data;
using DevLog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DevLog.Infrastructure.Repositories;

public class TagRepository : ITagRepository
{
    private readonly ApplicationDbContext _context;

    public TagRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExistsAsync(string Name, CancellationToken cancellationToken)
    {
        return await _context.Tags.AnyAsync(t => t.Name == Name, cancellationToken);
    }

    public async Task<Tag> CreateAsync(Tag tag, CancellationToken cancellationToken)
    {
        tag.CreatedAt = DateTime.UtcNow;
        await _context.Tags.AddAsync(tag, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return tag;
    }

    public async Task<Tag?> GetByIdAsync(Guid Id, CancellationToken cancellationToken)
    {
        return await _context.Tags.FirstOrDefaultAsync(t => t.Id == Id, cancellationToken);
    }

    public async Task<List<Tag>> GetByIdsAsync(List<Guid> tagIds, CancellationToken cancellationToken)
    {
        return await _context.Tags.Where( t => tagIds.Contains(t.Id)).ToListAsync(cancellationToken);
    }

    public async Task<Tag> UpdateAsync(Tag tag, CancellationToken cancellationToken)
    {
        _context.Tags.Update(tag);
        await _context.SaveChangesAsync(cancellationToken);
        return tag;
    }

    public async Task< (List<Tag>Tags, int TotalCount)> GetAllAsync(int Page, int PageSize, CancellationToken cancellationToken)
    {
        var TotalCount = await _context.Tags.CountAsync();

        var Tags = await _context.Tags
        .OrderBy(t => t.IsCustom) 
        .ThenBy(t => t.Name)
        .Skip((Page - 1 ) * PageSize) // skip previous pages
        .Take(PageSize) //only return current page
        .ToListAsync();

        return (Tags, TotalCount);
    }
}    