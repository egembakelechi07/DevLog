using DevLog.Domain.Entities;

namespace DevLog.Application.DTOs;


public sealed class LogDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;

    // public string Status { get; set; } = string.Empty;
    public Guid UserId { get; set; }        // just the ID,
    public List<TagDto> Tags { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? LastUpdatedAt { get; set; }
}

public class CreateLogDto
{
     public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Category Category { get; set; }
    public List<Guid> TagIds { get; set; } = new();
    
}

public class UpdateLogDto
{
     public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Category Category { get; set; }
    public List<Guid> TagIds { get; set; } = new();
    
}