using Assignment4.Core;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Assignment4.Entities;

public sealed class TagRepository : ITagRepository
{
    private readonly KanbanContext _context;

    public TagRepository(KanbanContext context)
    {
        _context = context;
    }

    public (Response Response, int TagId) Create(TagCreateDTO tag)
    {
        var entity = _context.Tags.FirstOrDefault(c => c.Name == tag.Name);
        Response response;

        if (entity is null)
        {
            entity = new Tag(tag.Name);

            _context.Tags.Add(entity);
            _context.SaveChanges();

            response = Response.Created;
        }
        else
        {
            response = Response.Conflict;
        }

        var created = new TagDTO(entity.Id, entity.Name);

        return (response, entity.Id);
    }

    public IReadOnlyCollection<TagDTO> ReadAll()
    {
        var tags = from c in _context.Tags
                     orderby c.Name
                     select new TagDTO(c.Id, c.Name);

        return tags.ToArray();
    }
    public TagDTO Read(int tagId)
    {
        var tags = from c in _context.Tags
                   where c.Id == tagId
                   select new TagDTO(c.Id, c.Name);

        try
        {
            return tags.First();
        }
        catch(Exception e)
        {
            return null;
        }

    }
    public Response Update(TagUpdateDTO tag)
    {
        var entity = _context.Tags.Find(tag.Id);
        Response response;

        if (entity is null)
        {
            response = Response.NotFound;
        }
        else if (_context.Tags.FirstOrDefault(c => c.Id != tag.Id && c.Name == tag.Name) != null)
        {
            response = Response.Conflict;
        }
        else
        {
            entity.Name = tag.Name;
            _context.SaveChanges();
            response = Response.Updated;
        }

        return response;
    }

    public Response Delete(int tagId, bool force = false)
    {
        var tag = _context.Tags.Include(c => c.WorkItems).FirstOrDefault(c => c.Id == tagId);
        Response response;

        if (tag is null)
        {
            response = Response.NotFound;
        }
        else if (tag.WorkItems.Any() && !force)
        {
            response = Response.Conflict;
        }
        else
        {
            _context.Tags.Remove(tag);
            _context.SaveChanges();

            response = Response.Deleted;
        }   

        return response;
    }
}
