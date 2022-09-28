using Assignment3.Core;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
        context.Tags.AddRange(new Tag("To Do") { Id = 1, WorkItems = new[] { new WorkItem("something") { Id = 2, Description = "something needs to be done", State = State.Active } } }, new Tag("Done") { Id = 2 });
        context.WorkItems.Add(new WorkItem("something") { Id = 1, Description = "something needs to be done", State = State.Active });
        context.SaveChanges();

        _context = context;
        _repository = new TagRepository(_context);
    }

    // Kanban Board Tests


    // Business Rules Tests

    [Fact]
    public void Create_given_Tag_returns_Created_with_Tag()
    {
        // Arrage & Act
        var (response, tagid) = _repository.Create(new TagCreateDTO("Doing"));

        // Assert
        response.Should().Be(Response.Created);
        tagid.Should().Be(3);
    }

    [Fact]
    public void Create_given_existing_Tag_returns_Conflict_with_existing_Tag()
    {
        // Arrange & Act
        var (response, tagid) = _repository.Create(new TagCreateDTO("To Do"));

        // Assert
        response.Should().Be(Response.Conflict);
        tagid.Should().Be(1);
    }

    [Fact]
    public void Read_given_non_existing_id_returns_null() => _repository.Read(42).Should().BeNull();

    [Fact]
    public void Read_given_existing_id_returns_tag() => _repository.Read(2).Should().Be(new TagDTO(2, "Done"));

    [Fact]
    public void ReadAll_returns_all_tags() => _repository.ReadAll().Should().BeEquivalentTo(new[] { new TagDTO(1, "To Do"), new TagDTO(2, "Done")});

    [Fact]
    public void Update_given_non_existing_Tag_returns_NotFound() => _repository.Update(new TagUpdateDTO(42, "Something")).Should().Be(Response.NotFound);

    [Fact]
    public void Update_given_existing_name_returns_Conflict_and_does_not_update()
    {
        // Arrange & Act
        var response = _repository.Update(new TagUpdateDTO(2, "To Do"));
        var entity = _context.Tags.Find(2)!;

        // Assert
        response.Should().Be(Response.Conflict);
        entity.Name.Should().Be("Done");
    }

    [Fact]
    public void Update_updates_and_returns_Updated()
    {
        // Arrange & Act
        var response = _repository.Update(new TagUpdateDTO(2, "New Name"));
        var entity = _context.Tags.Find(2)!;

        // Assert
        response.Should().Be(Response.Updated);
        entity.Name.Should().Be("New Name");
    }

    [Fact]
    public void Delete_given_non_existing_Id_returns_NotFound() => _repository.Delete(42).Should().Be(Response.NotFound);

    [Fact]
    public void Delete_deletes_and_returns_Deleted()
    {
        // Arrange & Act
        var response = _repository.Delete(2);
        var entity = _context.Tags.Find(2);

        // Assert
        response.Should().Be(Response.Deleted);
        entity.Should().BeNull();
    }

    [Fact]
    public void Delete_given_existing_Tags_with_Users_returns_Conflict_and_does_not_delete()
    {
        // Arrange & Act
        var response = _repository.Delete(1);

        // Assert
        response.Should().Be(Response.Conflict);
        _context.Tags.Find(1).Should().NotBeNull();
    }

    [Fact]
    public void Delete_given_existing_Tags_with_Users_and_does_delete()
    {
        // Arrange & Act
        var response = _repository.Delete(1, force: true);

        // Assert
        response.Should().Be(Response.Deleted);
        _context.Tags.Find(1).Should().BeNull();
    }


    public void Dispose()
    {
        _context.Dispose();
    }
}
