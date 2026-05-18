using System.ComponentModel;
using System.Formats.Asn1;
using System.Runtime.CompilerServices;

namespace DevLog.Domain.Entities;

public sealed class Log
{
    public Guid Id {get; set;}
    public string Title {get; set;} = string.Empty;
    public string Description {get; set;} = string.Empty;
    public Category Category {get; set;}
    public Status Status {get; set;}
    public Guid UserId {get; set;}
    public User User {get; set;} = null! ;
    public ICollection<Tag> Tags {get; set;} = new List<Tag>();
    public DateTime CreatedAt {get; set;} = DateTime.UtcNow;
    public DateTime LastUpdatedAt {get; set;} = DateTime.UtcNow;
}

public enum Category
{
    Feature = 1,
    BugFix = 2,
    Review = 3,
    Meeting = 4,
    Learning = 5,
    Documentation = 6
}

public enum Status
{
    Pending = 1,
    Draft = 2,
    Completed = 3
}

