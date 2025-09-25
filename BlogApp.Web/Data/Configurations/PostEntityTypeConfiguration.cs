using BlogApp.Web.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogApp.Web.Data.Configurations;

public class PostEntityTypeConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.Property(b => b.Title)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(b => b.Body)
            .HasMaxLength(10000);

        builder.Property(b => b.DatePosted)
            .HasDefaultValueSql("GETDATE()");

        builder.HasIndex(b => b.DatePosted);

        builder.HasIndex(b => b.Slug)
            .IsUnique();

        builder.Property(b => b.Slug)
            .IsRequired();
    }
}
