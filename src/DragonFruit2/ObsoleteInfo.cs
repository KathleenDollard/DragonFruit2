namespace DragonFruit2;

public enum ObsoleteStatus
{
    NotObsolete,
    Obsolete,
    Deprecated,
    ScheduledForRemoval
}

public class ObsoleteInfo
{
    // TODO: design this class. 
    public ObsoleteStatus IsObsolete;
    public required string Message { get; set; }
    public DateTime? RemoveOn { get; set; }
    
}
