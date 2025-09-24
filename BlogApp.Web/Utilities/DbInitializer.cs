using BlogApp.Web.Data;
using BlogApp.Web.Models;

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
                PostId = 1,
                Title = "First Post",
                Body = "This is the first post.",
                DatePosted = DateTime.UtcNow
            },
            new Post
            {
                PostId = 2,
                Title = "Second Post",
                Body = "This is the second post.",
                DatePosted = DateTime.UtcNow
            }
        ];
        context.Posts.AddRange(posts);

        Comment[] comments = [
            new Comment
            {
                CommentId = 1,
                PostId = 1,
                Name = "Alice",
                Text = "Great post!",
                DatePosted = DateTime.UtcNow
            },
            new Comment
            {
                CommentId = 2,
                PostId = 2,
                Name = "Bob",
                Text = "Thanks for sharing.",
                DatePosted = DateTime.UtcNow
            },
        ];
        context.Comments.AddRange(comments);

        context.SaveChanges();
    }
}
