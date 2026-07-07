using DevLog.Domain.Interfaces;
using DevLog.Infrastructure.Data;
using DevLog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Formats.Asn1;

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
        await _context.Tags.AddAsync(tag);
        await _context.SaveChangesAsync();
        return tag;
    }

    public async Task<Tag?> GetByIdAsync(Guid Id, CancellationToken cancellationToken)
    {
        return await _context.Tags.FirstOrDefaultAsync(t => t.Id == Id, cancellationToken);
    }

    public async Task<Tag> UpdateAsync(Tag tag, CancellationToken cancellationToken)
    {
        tag.UpdatedAt = DateTime.UtcNow;
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

        return (Tags, TotalCount) ;
    }
}    