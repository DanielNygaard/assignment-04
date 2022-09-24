using Microsoft.EntityFrameworkCore;
using Assignment3;

namespace Assignment3.Entities;

public class KanbanContext : DbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Task>(entity =>
        {
            entity.Property(e => e.Title).HasMaxLength(100);
            entity.Property(e => e.Title).IsRequired();

            entity.Property(e => e.Description).HasMaxLength(max);
            entity.Property(entity => e.Description).IsRequired(false);

            entity.Property(e => e.State).HasConversion(v => v.ToString(),v => (EnumState)Enum.Parse(typeof(EnumState),v));
            

        });

        modelBuilder.Entity<User>(entity =>
        {  
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Name).IsRequired();

            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired();
            entity.Property(e => e.Email).IsUnique();

        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.Name).IsUnique();

        });
    }

}
