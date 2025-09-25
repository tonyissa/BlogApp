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

        Comment[] comments = [
            new Comment
            {
                PostId = posts[0].PostId,
                Name = "Alice",
                Text = "Great post!",
                DatePosted = DateTime.UtcNow
            },
            new Comment
            {
                PostId = posts[1].PostId,
                Name = "Bob",
                Text = "Thanks for sharing.",
                DatePosted = DateTime.UtcNow
            },
        ];

        context.Comments.AddRange(comments);
        context.SaveChanges();
    }
}
