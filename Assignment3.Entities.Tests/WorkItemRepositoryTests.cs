using Assignment3.Core;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Assignment3.Entities.Tests;

public class WorkItemRepositoryTests
{
    private readonly KanbanContext _context;
    private readonly TagRepository _repository;

    public WorkItemRepositoryTests()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<KanbanContext>();
        builder.UseSqlite(connection);
        var context = new KanbanContext(builder.Options);
        context.Database.EnsureCreated();
        context.WorkItems.AddRange(
            new WorkItem("Make Pasta") { AssignedTo = new User() { Name = "Adrian"}, Created = DateTime.Now, Description = "We need to cook some pasta", Id = 1, State = State.Active, StateUpdated = DateTime.Now, Tags = new[] { new Tag("Doing")}},
            new WorkItem("Make Rice") { AssignedTo = new User() { Name = "Anna" }, Created = DateTime.Now, Description = "We need to cook some rice", Id = 2, State = State.New, StateUpdated = DateTime.Now, Tags = new[] { new Tag("To Do") } }
            );
        context.SaveChanges();

        _context = context;
        _repository = new TagRepository(_context);
    }


    public void Dispose()
    {
        _context.Dispose();
    }
}
