using BlogApp.Web.Data;
using BlogApp.Web.Data.DTOs;
using BlogApp.Web.Models;
using System.Security.Cryptography;
using System.Text;

namespace BlogApp.Web.Utilities;

public class DbInitializer
{
    public void Initialize(BlogContext context)
    {
        if (context.Posts.Any())
            return;

        Post[] posts =
        [
            new Post
            {
                Title = "First Post",
                Body = "This is the first post.",
                DatePosted = DateTime.UtcNow,
                Slug = "first-post"
            },
            new Post
            {
                Title = "Second Post",
                Body = "This is the second post.",
                DatePosted = DateTime.UtcNow,
                Slug = "second-post"
            }
        ];
        context.Posts.AddRange(posts);
        context.SaveChanges();

        var now = DateTime.UtcNow.AddMinutes(5);

        Comment[] comments = [
            new Comment
            {
                PostId = posts[0].PostId,
                Name = "Alice",
                Text = "Great post!",
                DatePosted = now,
                Token = GenerateToken(new CommentDTO { Name = "Alice", Text = "Great post!", DatePosted = now })
            },
            new Comment
            {
                PostId = posts[1].PostId,
                Name = "Bob",
                Text = "Thanks for sharing.",
                DatePosted = now,
                Token = GenerateToken(new CommentDTO { Name = "Bob", Text = "Thanks for sharing.", DatePosted = now })
            },
        ];

        context.Comments.AddRange(comments);
        context.SaveChanges();
    }

    public static string GenerateToken(CommentDTO comment)
    {
        var input = $"{comment.Name}{comment.Text}{comment.DatePosted.Ticks}";
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToBase64String(bytes).Replace("/", "_").Replace("+", "-");
    }
}
