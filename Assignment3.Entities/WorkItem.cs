using Assignment3.Core;
using System.Xml.Linq;

namespace Assignment3.Entities;

public class WorkItem
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public User? AssignedTo { get; set; }
    public string? Description { get; set; }
    public State State{ get; set; }
    public virtual ICollection<Tag>? Tags { get; set; }
    public DateTime Created { get; set; }
    public DateTime StateUpdated { get; set; }

    public WorkItem(string title)
    {
        Tags = new List<Tag>();
        Title = title;
    }

}
