namespace Assignment3.Entities;

public class User
{
    int Id { get; set; }
    string? Name { get; set; } 
    string? Email { get; set; }
    List<Task>? tasks { get; set; }
}
