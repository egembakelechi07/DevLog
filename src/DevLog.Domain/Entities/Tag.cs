namespace DevLog.Domain.Entities;

public sealed class Tag
{
    public Guid Id {get; set;}
    public string Name {get; set;} = string.Empty;
}