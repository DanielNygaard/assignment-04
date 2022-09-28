using Assignment3.Core;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Assignment3.Entities.Tests;

public class WorkItemRepositoryTests
{
    private readonly KanbanContext _context;
    private readonly WorkItemRepository _repository;

    public WorkItemRepositoryTests()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<KanbanContext>();
        builder.UseSqlite(connection);
        var context = new KanbanContext(builder.Options);
        context.Database.EnsureCreated();

        User user1 = new User() { Name = "Adrian", Id = 1, Email = "something@something.com" };
        User user2 = new User() { Name = "Anna", Id = 2, Email = "something2@something.com" };

        context.Users.Add(user1);
        context.Users.Add(user2);

        Tag tag1 = new Tag("Doing");
        Tag tag2 = new Tag("To Do");
        Tag tag3 = new Tag("Some Tag");
        Tag tag4 = new Tag("Some Tag2");
        Tag tag5 = new Tag("Some Tag3");

        context.Tags.Add(tag1);
        context.Tags.Add(tag2);
        context.Tags.Add(tag3);
        context.Tags.Add(tag4);
        context.Tags.Add(tag5);

        context.WorkItems.AddRange(
            new WorkItem("Make Pasta") { AssignedTo = user1, Created = DateTime.Now, Description = "We need to cook some pasta", Id = 1, State = State.Active, StateUpdated = DateTime.Now, Tags = new[] { tag1 }},
            new WorkItem("Make Rice") { AssignedTo = user2, Created = DateTime.Now, Description = "We need to cook some rice", Id = 2, State = State.New, StateUpdated = DateTime.Now, Tags = new[] { tag2 } }
            );
        
        context.SaveChanges();

        _context = context;
        _repository = new WorkItemRepository(_context);
    }

    [Fact]
    public void Delete_Tries_To_Delete_But_Fails_Because_State_Is_Active()
    {
        var response = _repository.Delete(1);

        response.Should().Be(Response.Conflict);

        var entity = _context.Tags.Find(1);

        entity.Should().NotBeNull();
    }

    [Fact]
    public void Delete_deletes_and_returns_Deleted()
    {
        var response = _repository.Delete(2);

        response.Should().Be(Response.Deleted);

        var entity = _context.WorkItems.Find(2);

        entity.Should().BeNull();
    }

    [Fact]
    public void Create_Item_State_Should_Be_New()
    {
        var response = _repository.Create(new WorkItemCreateDTO("Play Computer", (new User() { Name = "Gandalf", Id = 3, Email = "something3@something.com" }).Id, "Gandalf must play computer", new[] {(new Tag("Must Do")).Name}));

        response.Should().Be((Response.Created, 3));

        var entity = _context.WorkItems.Find(response.WorkItemId);

        entity.State.Should().Be(State.New);
    }

    [Fact]
    public void Create_And_Update_WorkItem_Must_Allow_For_Editing_Tags()
    {
        User user = new User() { Name = "Test", Id = 3, Email = "something3@something.com" };
        User user2 = new User() { Name = "Test", Id = 4, Email = "something4@something.com" };

        _context.Users.Add(user);
        _context.Users.Add(user2);

        var response = _repository.Create(new WorkItemCreateDTO("Test", user.Id, "Testing a description", new[] { (new Tag("Some Tag")).Name }));

        response.Should().Be((Response.Created, 3));

        var entity = _context.WorkItems.Find(response.WorkItemId);

        entity.Tags.Should().BeEquivalentTo(_context.Tags.Where(x => x.Name == "Some Tag"));

        var response2 = _repository.Update(new WorkItemUpdateDTO(response.WorkItemId, "Test", user2.Id, "Testing a description", new[] { (new Tag("Some Tag2")).Name, (new Tag("Some Tag3")).Name }, State.Active));

        response2.Should().Be((Response.Updated));

        var entity2 = _context.WorkItems.Find(response.WorkItemId);

        entity2.Tags.Should().BeEquivalentTo(new[] { _context.Tags.Where(x => x.Name == "Some Tag2").First(), _context.Tags.Where(x => x.Name == "Some Tag3").First() });

    }

    [Fact]

    public void Update_State_Should_Change_StateUpdated()
    {
        var entity = _context.WorkItems.Find(1);

        var initialStateUpdated = entity.StateUpdated;

        var response = _repository.Update(new WorkItemUpdateDTO(entity.Id, entity.Title, entity.AssignedTo.Id, entity.Description, entity.Tags.Select(x => x.Name).ToArray(), State.Closed));

        response.Should().Be(Response.Updated);

        entity.StateUpdated.Should().NotBe(initialStateUpdated);
    }

    [Fact]
    public void Assigning_User_Dosnt_Exist_Returns_Bad_Request()
    {
        var entity = _context.WorkItems.Find(1);

        var response = _repository.Update(new WorkItemUpdateDTO(entity.Id, entity.Title, 45, entity.Description, entity.Tags.Select(x => x.Name).ToArray(), State.Closed));

        response.Should().Be(Response.BadRequest);
    }



    public void Dispose()
    {
        _context.Dispose();
    }
}
