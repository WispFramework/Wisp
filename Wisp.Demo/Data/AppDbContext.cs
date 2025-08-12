using Microsoft.EntityFrameworkCore;
using Wisp.Demo.Data.Models;

namespace Wisp.Demo.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    
    public DbSet<Post> Posts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var user = new User
        {
            Id = 1,
            Username = "admin",
            Password = "$2a$11$5btcGkcxRRkAc1xoqY.5.u./yDYLcXO5H9tU8dHdKcH2zPmLwq1rm",
            Role = "admin"
        };
        
        modelBuilder.Entity<User>()
            .HasData(user);

        modelBuilder.Entity<Post>()
            .HasOne(p => p.Author)
            .WithMany(a => a.Posts);

        var dt = new DateTime(2025, 12, 12, 12, 12, 12, DateTimeKind.Utc);
        
        modelBuilder.Entity<Post>()
            .HasData(new Post
            {
                Id = 1,
                Title = "Wisp Demo",
                AuthorId = 1,
                Content = "Lorem ipsum dolor",
                PublishDate = dt,
                Slug = "wisp-demo"
            });
    }
}