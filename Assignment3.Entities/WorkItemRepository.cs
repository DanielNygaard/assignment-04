using Assignment3.Core;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Assignment3.Entities;

public sealed class WorkItemRepository : IWorkItemRepository
{

    private readonly KanbanContext _context;

    public WorkItemRepository(KanbanContext context)
    {
        _context = context;
    }

    public (Response Response, int WorkItemId) Create(WorkItemCreateDTO workItem)
    {
        var entity = _context.WorkItems.FirstOrDefault(c => c.Title == workItem.Title);
        Response response;

        if (!_context.Users.Contains(new User() { Id = (int)workItem.AssignedToId }) && entity is null)
        {
            response = Response.BadRequest;
        }
        else if (entity is null)
        {
            entity = new WorkItem(workItem.Title) { State = State.New, Created = DateTime.Now, StateUpdated = DateTime.Now};

            _context.WorkItems.Add(entity);
            _context.SaveChanges();

            response = Response.Created;
        }
        else
        {
            response = Response.Conflict;
        }

        var created = new WorkItemDTO(entity.Id, entity.Title, entity.AssignedTo.Name, entity.Tags.Select(x => x.Name).ToArray(), entity.State);

        return (response, entity.Id);
    }

    public (Response Response, int WorkItemId) Create(WorkItemCreateDTO workItem, ICollection<Tag> tags)
    {
        var entity = _context.WorkItems.FirstOrDefault(c => c.Title == workItem.Title);
        Response response;

        if (entity is null)
        {
            entity = new WorkItem(workItem.Title) { State = State.New, Created = DateTime.Now, StateUpdated = DateTime.Now, Tags = tags};

            _context.WorkItems.Add(entity);
            _context.SaveChanges();

            response = Response.Created;
        }
        else
        {
            response = Response.Conflict;
        }

        var created = new WorkItemDTO(entity.Id, entity.Title, entity.AssignedTo.Name, entity.Tags.Select(x => x.Name).ToArray(), entity.State);

        return (response, entity.Id);
    }

    public IReadOnlyCollection<WorkItemDTO> ReadAll()
    {
        var workItems = from c in _context.WorkItems
                   orderby c.Title
                   select new WorkItemDTO(c.Id, c.Title, c.AssignedTo.Name, c.Tags.Select(x => x.Name).ToArray(), c.State);

        return workItems.ToArray();
    }

    //public IReadOnlyCollection<WorkItemDTO> ReadAllRemoved()
    //{

    //}

    public IReadOnlyCollection<WorkItemDTO> ReadAllByTag(string tag)
    {
        var workItems = from c in _context.WorkItems
                   orderby c.Title
                   where c.Tags.Select(x => x.Name).Contains(tag)
                   select new WorkItemDTO(c.Id, c.Title, c.AssignedTo.Name, c.Tags.Select(x => x.Name).ToArray(), c.State);

        return workItems.ToArray();
    }
    public IReadOnlyCollection<WorkItemDTO> ReadAllByUser(int userId)
    {
        var workItems = from c in _context.WorkItems
                        orderby c.Title
                        where c.AssignedTo.Id == userId
                        select new WorkItemDTO(c.Id, c.Title, c.AssignedTo.Name, c.Tags.Select(x => x.Name).ToArray(), c.State);

        return workItems.ToArray();
    }
    public IReadOnlyCollection<WorkItemDTO> ReadAllByState(State state)
    {
        var workItems = from c in _context.WorkItems
                        orderby c.Title
                        where c.State == state
                        select new WorkItemDTO(c.Id, c.Title, c.AssignedTo.Name, c.Tags.Select(x => x.Name).ToArray(), c.State);

        return workItems.ToArray();
    }
    public WorkItemDetailsDTO Read(int workItemId)
    {
        var workItem = from c in _context.WorkItems
                        orderby c.Title
                        where c.Id == workItemId
                        select new WorkItemDetailsDTO(c.Id, c.Title, c.Description, c.Created, c.AssignedTo.Name, c.Tags.Select(x => x.Name).ToArray(), c.State, c.StateUpdated);

        return workItem.First();
    }
    public Response Update(WorkItemUpdateDTO workItem)
    {
        var entity = _context.WorkItems.Find(workItem.Id);
        Response response;

        if (!_context.Users.Contains(new User() { Id = (int)workItem.AssignedToId }) && entity is not null)
        {
            response = Response.BadRequest;
        }
        else if (entity is null)
        {
            response = Response.NotFound;
        }
        else if (_context.WorkItems.FirstOrDefault(c => c.Id != workItem.Id && c.Title == workItem.Title) != null)
        {
            response = Response.Conflict;
        }
        else
        {
            entity.Title = workItem.Title;
            entity.Description = workItem.Description;
            entity.Tags = (ICollection<Tag>?)workItem.Tags;
            entity.AssignedTo = _context.Users.Find(workItem.AssignedToId);

            if(entity.State != workItem.State)
            {
                entity.State = workItem.State;
                entity.StateUpdated = DateTime.Now;
            }

            _context.SaveChanges();
            response = Response.Updated;
        }

        return response;
    }

    public Response Delete(int workItemId)
    {
        var workitem = _context.WorkItems.Include(c => c.Tags).FirstOrDefault(c => c.Id == workItemId);
        Response response;

        if (workitem is null)
        {
            response = Response.NotFound;
        }
        else if (workitem.State != State.New)
        {
            response = Response.Conflict;
        }
        else
        {
            _context.WorkItems.Remove(workitem);
            _context.SaveChanges();

            workitem.State = State.Removed;
            response = Response.Deleted;
        }

        return response;
    }
}
