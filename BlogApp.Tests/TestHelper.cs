using AutoFixture;
using BlogApp.Tests.Helpers;
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
    public static readonly Fixture Fixture = IniitalizeFicture();

    private static Fixture IniitalizeFicture()
    {
        var fixture = new Fixture();
        fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => fixture.Behaviors.Remove(b));
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        return fixture;
    }

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
            var post = Fixture.Build<Post>()
                .With(p => p.PostId, i + 1)
                .With(p => p.Title, $"Test Post {i + 1}")
                .With(p => p.Body, "This is a test post body.")
                .With(p => p.DatePosted, DateTime.UtcNow.AddDays(-i))
                .With(p => p.Slug, $"test-post-{i + 1}")
                .Create();
            posts.Add(post);
        }

        return posts;
    }

    public static List<Comment> CreateComments(int countOfPosts, int count)
    {
        var comments = new List<Comment>();
        int idCounter = 1;

        for (int i = 0; i < countOfPosts; i++)
        {
            for (int j = 0; j < count; j++)
            {
                var comment = Fixture.Build<Comment>()
                    .With(c => c.CommentId, idCounter++)
                    .With(c => c.PostId, i + 1)
                    .With(c => c.Name, $"Author {j + 1}")
                    .With(c => c.Text, "This is a test comment.")
                    .With(c => c.DatePosted, DateTime.UtcNow.AddDays(-j))
                    .Create();
                comment.Token = TokenHelper.GenerateToken(comment.MapToObject());
                comments.Add(comment);  
            }
        }

        return comments;
    }
}