namespace DevLog.Domain.Entities;

public sealed class LogTags
{
    public Guid LogId {get; set;}
    public Log Log {get; set;} = null!;
    public Guid TagId {get; set;}
    public Tag Tag {get; set;} = null!;

}