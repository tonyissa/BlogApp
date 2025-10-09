using BlogApp.Web.Data;
using BlogApp.Web.Models;
using BlogApp.Web.Models.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace BlogApp.Tests.Integration;

[Trait("Category", "Integration")]
public class BlogIntegrationTests : IClassFixture<SqlServerContainerFixture>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly BlogApplicationFactory _app;
    private readonly SqlServerContainerFixture _containerFixture;

    public BlogIntegrationTests(SqlServerContainerFixture containerFixture)
    {
        _containerFixture = containerFixture;
        _app = new BlogApplicationFactory(_containerFixture.Container.GetConnectionString());
        _client = _app.CreateClient();
    }

    public async Task InitializeAsync() => await _app.ResetDatabaseAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetPost_WithInvalidSlug_ShouldReturnNotFound()
    {
        // Arrange
        var posts = TestHelper.CreatePosts(1);
        using var scope = _app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BlogContext>();
        context.Posts.AddRange(posts);
        await context.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync("/posts/non-existant-slug");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Index_WithSeededData_ShouldReturnViewWithAllPosts()
    {
        // Arrange
        var posts = TestHelper.CreatePosts(5);
        using var scope = _app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BlogContext>();
        context.Posts.AddRange(posts);
        await context.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync("/");
        var resultString = await response.Content.ReadAsStringAsync();

        // Assert
        foreach (var post in posts)
        {
            Assert.Contains(post.Title, resultString);
        }
    }

    [Fact]
    public async Task CreatePost_WithInvalidAdminKey_ShouldReturnViewWithAdminKeyError()
    {
        // Arrange
        var post = TestHelper.CreatePosts(1).First();
        var createPostVM = new CreatePostViewModel()
        {
            Title = post.Title,
            Body = post.Body,
            AdminKey = "invalid-admin-key"
        };
        var formData = TestHelper.MapToFormEncodedData(createPostVM);

        // Act
        var response = await _client.PostAsync("/posts/create", formData);
        var resultString = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Contains("Invalid admin key.", resultString);
    }

    [Fact]
    public async Task CreatePost_WithValidAdminKey_ShouldReturnViewWithItem()
    {
        // Arrange
        var post = TestHelper.CreatePosts(1).First();
        var createPostVM = new CreatePostViewModel()
        {
            Title = post.Title,
            Body = post.Body,
            AdminKey = "test-admin-key"
        };
        var formData = TestHelper.MapToFormEncodedData(createPostVM);

        // Act
        var response = await _client.PostAsync("/posts/create", formData);
        var resultString = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Contains(post.Title, resultString);
    }

    [Fact]
    public async Task CreatePost_WithDuplicateSlug_ShouldReturnViewWithModelValidationErrors()
    {
        // Arrange
        var post = TestHelper.CreatePosts(1).First();
        using var scope = _app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BlogContext>();
        context.Posts.Add(post);
        await context.SaveChangesAsync();

        var createPostVM = new CreatePostViewModel()
        {
            Title = post.Title,
            Body = post.Body,
            AdminKey = "test-admin-key"
        };
        var formData = TestHelper.MapToFormEncodedData(createPostVM);

        // Act
        var response = await _client.PostAsync("/posts/create", formData);
        var resultString = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Contains("An error occurred:", resultString);
    }

    [Fact]
    public async Task DeletePost_WithValidAdminKeyAndSlug_ShouldReturnViewWithoutDeletedItem()
    {
        // Arrange
        var posts = TestHelper.CreatePosts(5);
        using var scope = _app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BlogContext>();
        context.Posts.AddRange(posts);
        await context.SaveChangesAsync();

        var postToDelete = posts.First();
        var deletePostVM = new DeletePostViewModel()
        {
            Title = postToDelete.Title,
            Slug = postToDelete.Slug,
            AdminKey = "test-admin-key"
        };
        var formData = TestHelper.MapToFormEncodedData(deletePostVM);

        // Act
        var response = await _client.PostAsync($"/posts/{postToDelete.Slug}/delete", formData);
        var resultString = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.DoesNotContain(postToDelete.Title, resultString);
    }

    [Fact]
    public async Task DeletePost_WithInvalidAdminKey_ShouldReturnViewWithAdminKeyError()
    {
        // Arrange
        var post = TestHelper.CreatePosts(1).First();
        using var scope = _app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BlogContext>();
        context.Posts.Add(post);
        await context.SaveChangesAsync();

        var deletePostVM = new DeletePostViewModel()
        {
            Title = post.Title,
            Slug = post.Slug,
            AdminKey = "invalid-admin-key"
        };
        var formData = TestHelper.MapToFormEncodedData(deletePostVM);

        // Act
        var response = await _client.PostAsync($"/posts/{post.Slug}/delete", formData);
        var resultString = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Contains("Invalid admin key.", resultString);
    }

    [Fact]
    public async Task DeletePost_WithInvalidSlug_ShouldReturnViewWithModelValidationErrors()
    {
        // Arrange
        var post = TestHelper.CreatePosts(1).First();
        using var scope = _app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BlogContext>();
        context.Posts.Add(post);
        await context.SaveChangesAsync();

        var deletePostVM = new DeletePostViewModel()
        {
            Title = post.Title,
            Slug = "invalid-slug",
            AdminKey = "test-admin-key"
        };
        var formData = TestHelper.MapToFormEncodedData(deletePostVM);

        // Act
        var response = await _client.PostAsync("/posts/invalid-slug/delete", formData);
        var resultString = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Contains("An error occurred:", resultString);
    }

    [Fact]
    public async Task CreateComment_ShouldReturnViewWithComment()
    {
        // Arrange
        var post = TestHelper.CreatePosts(1).First();
        using var scope = _app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BlogContext>();
        context.Posts.Add(post);
        await context.SaveChangesAsync();

        var comment = TestHelper.CreateComments(1, post.PostId).First();
        var createCommentVM = new CreateCommentViewModel()
        {
            CommentName = comment.Name,
            CommentText = comment.Text,
            Slug = post.Slug
        };
        var formData = TestHelper.MapToFormEncodedData(createCommentVM);

        // Act
        var response = await _client.PostAsync($"/posts/{post.Slug}/comment", formData);
        var resultString = await response.Content.ReadAsStringAsync();
        var postedComment = context.Comments.First(c => c.PostId == post.PostId);

        // Assert
        Assert.Contains(postedComment.Token, resultString);
    }

    [Fact]
    public async Task DeleteComment_WithInvalidAdminKey_ShouldReturnViewWithAdminKeyError()
    {
        // Arrange
        var post = TestHelper.CreatePosts(1).First();
        var comment = TestHelper.CreateComments(1, 1).First();
        using var scope = _app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BlogContext>();
        context.Posts.Add(post);
        await context.SaveChangesAsync();
        context.Comments.Add(comment);
        await context.SaveChangesAsync();

        var deleteCommentVM = new DeleteCommentViewModel()
        {
            Token = comment.Token,
            Author = comment.Name,
            Comment = comment.Text,
            AdminKey = "invalid-admin-key"
        };
        var formData = TestHelper.MapToFormEncodedData(deleteCommentVM);

        // Act
        var response = await _client.PostAsync($"/posts/{post.Slug}/delete-comment/{comment.Token}", formData);
        var resultString = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Contains("Invalid admin key.", resultString);
    }

    [Fact]
    public async Task DeleteComment_WithValidAdminKey_ShouldReturnViewWithoutComment()
    {
        // Arrange
        var post = TestHelper.CreatePosts(1).First();
        var comment = TestHelper.CreateComments(1, 1).First();
        using var scope = _app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BlogContext>();
        context.Posts.Add(post);
        await context.SaveChangesAsync();
        context.Comments.Add(comment);
        await context.SaveChangesAsync();

        var deleteCommentVM = new DeleteCommentViewModel()
        {
            Token = comment.Token,
            Author = comment.Name,
            Comment = comment.Text,
            AdminKey = "test-admin-key",
            DatePosted = comment.DatePosted
        };
        var formData = TestHelper.MapToFormEncodedData(deleteCommentVM);

        // Act
        var response = await _client.PostAsync($"/posts/{post.Slug}/delete-comment/{comment.Token}", formData);
        var resultString = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Contains(post.Title, resultString);
        Assert.DoesNotContain(comment.Token, resultString);
    }
}