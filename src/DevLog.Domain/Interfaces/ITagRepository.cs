using DevLog.Domain.Entities;
namespace DevLog.Domain.Interfaces;



public interface ITagRepository
{
    Task<bool> ExistsAsync(string Name, CancellationToken cancellationToken);
    Task<Tag> CreateAsync(Tag tag, CancellationToken cancellationToken);
    Task<Tag?> GetByIdAsync(Guid Id, CancellationToken cancellationToken);
    Task<Tag> UpdateAsync(Tag tag, CancellationToken cancellationToken);
    Task< (List<Tag>Tags, int TotalCount)> GetAllAsync(int Page, int PageSize, CancellationToken cancellationToken);
}