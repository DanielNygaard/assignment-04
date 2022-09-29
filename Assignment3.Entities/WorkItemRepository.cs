using Assignment3.Core;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;
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

        if (_context.Users.Contains(new User() { Id = (int)workItem.AssignedToId }) && entity is null)
        {
            response = Response.BadRequest;
            return (response, 0);
        }
        else if (entity is null)
        {
            entity = new WorkItem(workItem.Title) { Id = _context.WorkItems.Count() + 1,State = State.New, Created = DateTime.UtcNow, StateUpdated = DateTime.UtcNow, Tags = _context.Tags.Where(x => workItem.Tags.Contains(x.Name)).ToList()};

            _context.WorkItems.Add(entity);
            _context.SaveChanges();
            response = Response.Created;
            
        }
        else
        {
            response = Response.Conflict;
        }

        return (response, entity.Id);
    }

    public IReadOnlyCollection<WorkItemDTO> ReadAll()
    {
        var workItems = from c in _context.WorkItems
                   orderby c.Title
                   select new WorkItemDTO(c.Id, c.Title, c.AssignedTo.Name, c.Tags.Select(x => x.Name).ToArray(), c.State);

        return workItems.ToArray();
    }

    public IReadOnlyCollection<WorkItemDTO> ReadAllRemoved()
    {
        var workItems = from c in _context.WorkItems
                        orderby c.Title
                        where c.State == State.Removed
                        select new WorkItemDTO(c.Id, c.Title, c.AssignedTo.Name, c.Tags.Select(x => x.Name).ToArray(), c.State);
                        
        return workItems.ToArray();
    }

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
    public WorkItemDetailsDTO? Read(int workItemId)
    {
        var workItem = from c in _context.WorkItems
                        orderby c.Title
                        where c.Id == workItemId
                        select new WorkItemDetailsDTO(c.Id, c.Title, c.Description, c.Created, c.AssignedTo.Name, c.Tags.Select(x => x.Name).ToArray(), c.State, c.StateUpdated);

        if(workItem.Count() == 0)
        {
            return null;
        }

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
            _context.WorkItems.Remove(entity);
            _context.SaveChanges();

            entity.Title = workItem.Title;
            entity.Description = workItem.Description;
            entity.Tags = _context.Tags.Where(x => workItem.Tags.Contains(x.Name)).ToList();
            entity.AssignedTo = _context.Users.Find(workItem.AssignedToId);

            if(entity.State != workItem.State)
            {
                entity.State = workItem.State;
                entity.StateUpdated = DateTime.UtcNow;
            }

            _context.WorkItems.Add(entity);
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
            workitem.State = State.Removed;
            _context.WorkItems.Remove(workitem);
            _context.SaveChanges();

            response = Response.Deleted;
        }

        return response;
    }
}
