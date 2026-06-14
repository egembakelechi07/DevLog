namespace DevLog.Domain.Entities;

public sealed class User
{
    public Guid Id {get; set;}
    public string Name {get; set;} = string.Empty;
    public string Email {get; set;} = string.Empty;
    public string PasswordHash {get; set;} = string.Empty;
    public string? RefreshToken {get; set;}
    public DateTime? RefreshTokenExpiresAt {get; set;}
    public bool IsDisabled {get; set;} = false;
    public DateTime CreatedAt {get; set;} = DateTime.UtcNow;
    public DateTime? LastUpdatedAt {get; set;}
    public ICollection<Log> Logs {get; set;} = new List<Log>();
}