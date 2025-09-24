using BlogApp.Web.Data.Configurations;
using BlogApp.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Web.Data;

public class BlogContext(DbContextOptions<BlogContext> options) : DbContext(options)
{
    public DbSet<Post> Posts { get; set; }
    public DbSet<Comment> Comments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PostEntityTypeConfiguration).Assembly);
    }
}
