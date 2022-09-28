
using Assignment3;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Assignment3.Core;

namespace Assignment3.Entities;

public sealed class KanbanContext : DbContext
{
    public DbSet<WorkItem> WorkItems => Set<WorkItem>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Tag> Tags => Set<Tag>();

    public KanbanContext(DbContextOptions<KanbanContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WorkItem>(entity =>
        {
            entity.Property(e => e.Title).HasMaxLength(100);
            entity.Property(e => e.Title).IsRequired();

            entity.Property(e => e.Description).HasMaxLength(int.MaxValue);
            entity.Property(e => e.Description).IsRequired(false);

            entity.Property(e => e.State).HasConversion(v => v.ToString(),v => (State)Enum.Parse(typeof(State),v));
    

        });

        modelBuilder.Entity<User>(entity =>
        {  
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Name).IsRequired();

            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired();
            entity.HasIndex(e => e.Email).IsUnique();

        });
         
        modelBuilder.Entity<Tag>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Name).IsRequired();
            entity.HasIndex(e => e.Name).IsUnique();

        });

        modelBuilder.Entity<WorkItem>()
            .HasMany<Tag>(wi => wi.Tags)
            .WithMany(t => t.WorkItems);

    }

}
