namespace DevLog.Domain.Entities;

public sealed class User
{
    public Guid Id {get; set;}
    public string Name {get; set;} = string.Empty;
    public string Email {get; set;} = string.Empty;
    public string PasswordHash {get; set;} = string.Empty;
    public DateTime CreatedAt {get; set;} = DateTime.UtcNow;
    public ICollection<Log> Logs {get; set;} = new List<Log>();
}