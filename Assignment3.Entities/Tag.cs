namespace Assignment3.Entities;

public class Tag
{
    public int Id { get; set; }
    public string Name { get; set; }
    public virtual ICollection<WorkItem>? WorkItems { get; set; }

    public Tag(String name)
    {
        WorkItems = new List<WorkItem>();
        Name = name;
    }
}
