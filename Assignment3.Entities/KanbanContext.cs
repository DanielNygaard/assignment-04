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

            entity.Property(e => e.State).HasConversion(v => v.ToString(),v => (EnumState)Enum.Parse(typeof(EnumState),v));

        });
    }


}
