using BlogApp.Web.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogApp.Web.Data.Configurations;

public class CommentEntityTypeConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.HasOne(c => c.Post)
            .WithMany(b => b.Comments)
            .HasForeignKey(c => c.PostId)
            .IsRequired();

        builder.Property(c => c.Text)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(c => c.DatePosted)
            .HasDefaultValueSql("GETDATE()");

        builder.Property(c => c.Name)
            .HasMaxLength(30)
            .IsRequired();

        builder.HasIndex(c => c.Token)
            .IsUnique();

        builder.Property(c => c.Token)
            .IsRequired();

        builder.Property(c => c.RowVersion)
            .IsRowVersion();
    }
}
