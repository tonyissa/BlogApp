using AutoFixture;
using BlogApp.Tests.Helpers;
using BlogApp.Web.Data.DTOs;
using BlogApp.Web.Extensions;
using BlogApp.Web.Models;
using BlogApp.Web.Services;
using Moq;

namespace BlogApp.Tests.Unit;

public class BlogServiceTests
{
    public static readonly Fixture Fixture = TestHelper.Fixture;

    [Fact]
    public async Task GetAllPostsAsync_ShouldReturnAllPostsOrderedByDateDescending()
    {
        // Arrange
        var posts = TestHelper.CreatePosts(5);
        var context = TestHelper.CreateMockBlogContext(posts: posts).Object;
        var service = TestHelper.CreateMockBlogService(context);

        // Act
        var result = await service.GetAllPostsAsync();
        var orderedResults = result.OrderByDescending(r => r.DatePosted);

        // Assert
        Assert.Equal(5, result.Count());
        Assert.True(result.SequenceEqual(orderedResults));
    }

    [Fact]
    public async Task GetPostAsync_WithValidSlug_ShouldReturnPost()
    {
        // Arrange
        var posts = TestHelper.CreatePosts(1);
        var context = TestHelper.CreateMockBlogContext(posts: posts).Object;
        var service = TestHelper.CreateMockBlogService(context);

        // Act
        var result = await service.GetPostAsync(posts[0].Slug);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(posts[0].Title, result.Title);
    }

    [Fact]
    public async Task GetPostAsync_WithInvalidSlug_ShouldReturnNull()
    {
        // Arrange
        var posts = TestHelper.CreatePosts(1);
        var context = TestHelper.CreateMockBlogContext(posts: posts).Object;
        var service = TestHelper.CreateMockBlogService(context);

        // Act
        var result = await service.GetPostAsync("NOT A SLUG");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddPostAsync_WithValidAdminKey_ShouldAddPost()
    {
        // Arrange
        var context = TestHelper.CreateMockBlogContext();
        var service = TestHelper.CreateMockBlogService(context.Object);
        var newPost = Fixture.Build<PostDTO>()
            .With(p => p.Title, "New Post")
            .With(p => p.Body, "This is a new post body.")
            .With(p => p.Slug, "new-post")
            .Create();

        // Act
        await service.AddPostAsync("test-admin-key", newPost);

        // Assert
        context.Verify(c => c.Posts.Add(It.IsAny<Post>()));
        context.Verify(c => c.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task AddPostAsync_WithoutValidAdminKey_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        var context = TestHelper.CreateMockBlogContext();
        var service = TestHelper.CreateMockBlogService(context.Object);
        var newPost = Fixture.Build<PostDTO>()
            .With(p => p.Title, "New Post")
            .With(p => p.Body, "This is a new post body.")
            .With(p => p.Slug, "new-post")
            .Create();

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => 
            await service.AddPostAsync("invalid-admin-key", newPost));
    }

    [Fact]
    public async Task DeletePostAsync_WithValidAdminKey_ShouldDeletePost()
    {
        // Arrange
        var posts = TestHelper.CreatePosts(1);
        var context = TestHelper.CreateMockBlogContext(posts: posts);
        var service = TestHelper.CreateMockBlogService(context.Object);

        // Act
        await service.DeletePostAsync("test-admin-key", posts[0].Slug);

        // Assert
        context.Verify(c => c.Posts.Remove(It.IsAny<Post>()));
        context.Verify(c => c.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task DeletePostAsync_WithoutValidAdminKey_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        var posts = TestHelper.CreatePosts(1);
        var context = TestHelper.CreateMockBlogContext(posts: posts);
        var service = TestHelper.CreateMockBlogService(context.Object);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            await service.DeletePostAsync("invalid-admin-key", posts[0].Slug));
    }

    [Fact]
    public async Task DeletePostAsync_WithInvalidSlug_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var posts = TestHelper.CreatePosts(1);
        var context = TestHelper.CreateMockBlogContext(posts: posts);
        var service = TestHelper.CreateMockBlogService(context.Object);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            await service.DeletePostAsync("test-admin-key", "invalid-slug"));
    }

    [Fact]
    public async Task DeleteCommentAsync_WithValidAdminKey_ShouldDeleteComment()
    {
        // Arrange
        var posts = TestHelper.CreatePosts(1);
        var comments = TestHelper.CreateComments(1, 1);
        var context = TestHelper.CreateMockBlogContext(posts, comments);
        var service = TestHelper.CreateMockBlogService(context.Object);

        // Act
        await service.DeleteCommentAsync("test-admin-key", comments[0].Token);

        // Assert
        context.Verify(c => c.Comments.Remove(It.IsAny<Comment>()));
        context.Verify(c => c.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task DeleteCommentAsync_WithoutValidAdminKey_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        var posts = TestHelper.CreatePosts(1);
        var comments = TestHelper.CreateComments(1, 1);
        var context = TestHelper.CreateMockBlogContext(posts, comments);
        var service = TestHelper.CreateMockBlogService(context.Object);
        var token = BlogService.GenerateToken(comments[0].MapToObject());

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            await service.DeleteCommentAsync("invalid-admin-key", token));
    }

    [Fact]
    public async Task DeleteCommentAsync_WithInvalidToken_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var posts = TestHelper.CreatePosts(1);
        var comments = TestHelper.CreateComments(1, 1);
        var context = TestHelper.CreateMockBlogContext(posts, comments);
        var service = TestHelper.CreateMockBlogService(context.Object);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            await service.DeleteCommentAsync("test-admin-key", "invalid-token"));
    }
}