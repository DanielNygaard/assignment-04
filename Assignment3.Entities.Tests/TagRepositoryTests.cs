using Assignment3.Core;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Assignment3.Entities.Tests;

public class TagRepositoryTests
{
    private readonly KanbanContext _context;
    private readonly TagRepository _repository;

    public TagRepositoryTests()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<KanbanContext>();
        builder.UseSqlite(connection);
        var context = new KanbanContext(builder.Options);
        context.Database.EnsureCreated();
        context.Tags.AddRange(new Tag("To Do") { Id = 1 }, new Tag("Done") { Id = 2 });
        context.WorkItems.Add(new WorkItem { Id = 1, Title = "something", AssignedTo = new User("Adrian"), Description = "something needs to be done", State = EnumState.Active });
        context.SaveChanges();

        _context = context;
        _repository = new TagRepository(_context);
    }

    [Fact]
    public void Create_given_Tag_returns_Created_with_Tag()
    {
        var (response, tagid) = _repository.Create(new TagCreateDTO("To Do"));

        response.Should().Be(Response.Created);

        tagid.Should().Be(0);
    }
}
