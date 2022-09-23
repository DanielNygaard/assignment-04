namespace Assignment3.Entities;

public class Tag
{
    int Id { get; set; }
    string Name { get; set; }
    public virtual ICollection<Task>? Tasks { get; set; }
}
