namespace DevLog.Application.DTOs;

public sealed class TagDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsCustom {get; set;}
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt {get; set;}
}

public sealed class CreateTagDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public sealed class UpdateTagDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}