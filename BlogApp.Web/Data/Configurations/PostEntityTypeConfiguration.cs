using BlogApp.Web.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogApp.Web.Data.Configurations;

public class PostEntityTypeConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.Property(p => p.Title)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(p => p.Body)
            .HasMaxLength(10000);

        builder.Property(p => p.DatePosted)
            .HasDefaultValueSql("GETDATE()");

        builder.HasIndex(p => p.DatePosted);

        builder.HasIndex(p => p.Slug)
            .IsUnique();

        builder.Property(p => p.Slug)
            .IsRequired();

        builder.Property(p => p.RowVersion)
            .IsRowVersion();

        builder.HasMany(p => p.Comments)
            .WithOne(c => c.Post)
            .HasForeignKey(c => c.PostId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
