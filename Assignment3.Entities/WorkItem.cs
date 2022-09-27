namespace Assignment3.Entities;

public class WorkItem
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public User? AssignedTo { get; set; }
    public string? Description { get; set; }
    public EnumState State{ get; set; }
    public virtual ICollection<Tag>? Tags { get; set; }
    
}

public enum EnumState { New, Active, Resolved, Closed, Removed }
