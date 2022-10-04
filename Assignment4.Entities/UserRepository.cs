using Assignment4.Core;

namespace Assignment4.Entities;

public class UserRepository : IUserRepository
{
    private readonly KanbanContext _context;

    public UserRepository(KanbanContext context)
    {
        _context = context;
    }

    (Response Response, int UserId) IUserRepository.Create(UserCreateDTO user) 
    {
        var entity = _context.Users.FirstOrDefault(u => u.Email == user.Email);

        Response response;

        if (entity is null)
        {
            entity = new User() { Name = user.Name, Email = user.Email,  };

            _context.Users.Add(entity);
            _context.SaveChanges();

            response = Response.Created;
        }
        else
        {
            response = Response.Conflict;
        }

        return (response, entity.Id);
    }

    IReadOnlyCollection<UserDTO> IUserRepository.ReadAll() 
    {
        List<UserDTO> list = new ();
        var entity = _context.Users;

        foreach (var e in entity) {
            list.Add(new UserDTO(e.Id, e.Name, e.Email));
        }
        
        return list;
    }

    UserDTO IUserRepository.Read(int userId) 
    {
        var entity = _context.Users.FirstOrDefault(u => u.Id == userId);

        return new UserDTO(userId, entity.Name, entity.Email); 
    }

    Response IUserRepository.Update(UserUpdateDTO user) 
    {
        var entity = _context.Users.FirstOrDefault(u => u.Id == user.Id);
        entity.Id = user.Id;
        entity.Name = user.Name;
        entity.Email = user.Email;

        if (entity is null) 
        {
            return Response.NotFound;
        }
        else
        {
            _context.Users.Update(entity);
            _context.SaveChanges();

            return Response.Updated;
        }
    }

    Response IUserRepository.Delete(int userId, bool force = false) 
    {
        var entity = _context.Users.FirstOrDefault(u => u.Id == userId);

        Response response;

        if (entity is null)
            response = Response.NotFound;
        else if (entity is null || force)
        {
            _context.Users.Remove(entity);
            _context.SaveChanges();

            response = Response.Deleted;
        }
        else response = Response.Conflict;

        return response;
    }
}
