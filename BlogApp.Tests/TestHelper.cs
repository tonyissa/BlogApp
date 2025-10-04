using BlogApp.Web.Data;
using BlogApp.Web.Extensions;
using BlogApp.Web.Interfaces;
using BlogApp.Web.Models;
using BlogApp.Web.Options;
using BlogApp.Web.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using Moq.EntityFrameworkCore;

namespace BlogApp.Tests;

public static class TestHelper
{

    public static Mock<BlogContext> CreateMockBlogContext(List<Post>? posts = null, List<Comment>? comments = null)
    {
        posts ??= [];
        comments ??= [];

        var mockContext = new Mock<BlogContext>(new DbContextOptionsBuilder<BlogContext>().Options);
        mockContext.Setup(c => c.Posts).ReturnsDbSet(posts);
        mockContext.Setup(c => c.Comments).ReturnsDbSet(comments);

        return mockContext;
    }

    public static IBlogService CreateMockBlogService(BlogContext context)
    {
        var mockAdminOptions = new Mock<IOptions<AdminOptions>>();
        mockAdminOptions.Setup(a => a.Value).Returns(new AdminOptions { Key = "test-admin-key" });
        return new BlogService(context, mockAdminOptions.Object);
    }

    // Data creation helpers
    public static List<Post> CreatePosts(int count)
    {
        var posts = new List<Post>();

        for (int i = 0; i < count; i++)
        {
            var post = new Post
            {
                Title = $"Test Post {i + 1}",
                Body = "This is a test post body.",
                DatePosted = DateTime.UtcNow.AddDays(-i),
                Slug = $"test-post-{i + 1}",
            };
            posts.Add(post);
        }

        return posts;
    }

    public static List<Comment> CreateComments(int count, int? optionalPostIdSet = null)
    {
        var comments = new List<Comment>();

        for (int i = 0; i < count; i++)
        {
            var comment = new Comment()
            {
                Name = $"Author {i + 1}",
                Text = "This is a test comment.",
                DatePosted = DateTime.UtcNow.AddDays(-i),
            };
            if (optionalPostIdSet != null) 
                comment.PostId = optionalPostIdSet.Value;
            comment.Token = BlogService.GenerateToken(comment.MapToObject());
            comments.Add(comment);  
        }

        return comments;
    }
}