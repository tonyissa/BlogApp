using BlogApp.Web.Data;
using BlogApp.Web.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

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

    //[Fact]
    //public async Task GetPost_WithValidSlug_ShouldReturnPost()
    //{
    //    // Arrange
    //    var posts = TestHelper.CreatePosts(1);
    //    using var scope = _app.Services.CreateScope();
    //    var context = scope.ServiceProvider.GetRequiredService<BlogContext>();
    //    context.Posts.AddRange(posts);
    //    await context.SaveChangesAsync();

    //    // Act
    //    var response = await _client.GetAsync($"/posts/{posts.First().Slug}");
    //    var responseString = await response.Content.ReadAsStringAsync();

    //    // Assert
    //    Assert.Contains(posts.First().Title, responseString);
    //}

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

    //[Fact]
    //public async Task Index_WithSeededData_ShouldReturnAllPosts()
    //{
    //    // Arrange
    //    var posts = TestHelper.CreatePosts(5);
    //    using var scope = _app.Services.CreateScope();
    //    var context = scope.ServiceProvider.GetRequiredService<BlogContext>();
    //    context.Posts.AddRange(posts);
    //    await context.SaveChangesAsync();

    //    // Act
    //    var response = await _client.GetAsync("/");
    //    var resultString = await response.Content.ReadAsStringAsync();

    //    // Assert
    //    foreach (var post in posts)
    //    {
    //        Assert.Contains(post.Title, resultString);
    //    }
    //}

    [Fact]
    public async Task CreatePost_WithInvalidAdminKey_ShouldReturnUnauthorized()
    {
        // Arrange
        var posts = TestHelper.CreatePosts(1);
        var postDTO = posts.First().MapToObject();
        _client.DefaultRequestHeaders.Add("admin_key", "invalid-admin-key");

        // Act
        var response = await _client.PostAsJsonAsync("/posts/create", postDTO);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    //[Fact]
    //public async Task CreatePost_WithValidAdminKey_ShouldReturnViewWithItem()
    //{
    //    // Arrange
    //    var posts = TestHelper.CreatePosts(1);
    //    var postDTO = posts.First().MapToObject();
    //    _client.DefaultRequestHeaders.Add("admin_key", "test-admin-key");

    //    // Act
    //    var response = await _client.PostAsJsonAsync("/posts/create", postDTO);
    //    var resultString = await response.Content.ReadAsStringAsync();

    //    // Assert
    //    Assert.Contains(postDTO.Title, resultString);
    //}

    //[Fact]
    //public async Task CreatePost_WithDuplicateSlug_ShouldReturnBadRequest()
    //{
    //    // Arrange
    //    var posts = TestHelper.CreatePosts(1);
    //    using var scope = _app.Services.CreateScope();
    //    var context = scope.ServiceProvider.GetRequiredService<BlogContext>();
    //    context.Posts.AddRange(posts);
    //    await context.SaveChangesAsync();
    //    var postDTO = posts.First().MapToObject();
    //    _client.DefaultRequestHeaders.Add("admin_key", "test-admin-key");

    //    // Act
    //    var response = await _client.PostAsJsonAsync("/posts/create", postDTO);

    //    // Assert
    //    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    //}

    //[Fact]
    //public async Task DeletePost_WithValidAdminKeyAndSlug_ShouldReturnViewWithoutDeletedItem()
    //{
    //    // Arrange
    //    var posts = TestHelper.CreatePosts(5);
    //    using var scope = _app.Services.CreateScope();
    //    var context = scope.ServiceProvider.GetRequiredService<BlogContext>();
    //    context.Posts.AddRange(posts);
    //    await context.SaveChangesAsync();
    //    _client.DefaultRequestHeaders.Add("admin_key", "test-admin-key");

    //    // Act
    //    var response = await _client.GetAsync($"/posts/{posts[^1].Slug}/");
    //    var resultString = await response.Content.ReadAsStringAsync();

    //    // Assert
    //    Assert.DoesNotContain(posts[^1].Title, resultString);
    //}

    [Fact]
    public async Task DeletePost_WithInvalidAdminKey_ShouldReturnUnauthorized()
    {
        // Arrange
        var posts = TestHelper.CreatePosts(1);
        using var scope = _app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BlogContext>();
        context.Posts.AddRange(posts);
        await context.SaveChangesAsync();
        _client.DefaultRequestHeaders.Add("admin_key", "invalid-admin-key");

        // Act
        var response = await _client.PostAsync($"/posts/{posts.First().Slug}/delete", null);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeletePost_WithInvalidSlug_ShouldReturnBadRequest()
    {
        // Arrange
        var posts = TestHelper.CreatePosts(1);
        using var scope = _app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BlogContext>();
        context.Posts.AddRange(posts);
        await context.SaveChangesAsync();
        _client.DefaultRequestHeaders.Add("admin_key", "test-admin-key");

        // Act
        var response = await _client.PostAsync("/posts/invalid-slug/delete", null);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    //[Fact]
    //public async Task CreateComment_ShouldReturnViewWithComment()
    //{
    //    // Arrange
    //    var posts = TestHelper.CreatePosts(1);
    //    using var scope = _app.Services.CreateScope();
    //    var context = scope.ServiceProvider.GetRequiredService<BlogContext>();
    //    context.Posts.AddRange(posts);
    //    await context.SaveChangesAsync();

    //    _client.DefaultRequestHeaders.Add("admin_key", "test-admin-key");
    //    var comments = TestHelper.CreateComments(1, posts.First().PostId);
    //    var commentDTO = comments.First().MapToObject();

    //    // Act
    //    var response = await _client.PostAsJsonAsync($"/posts/{posts.First().Slug}/create-comment", commentDTO);
    //    var resultString = await response.Content.ReadAsStringAsync();

    //    // Assert
    //    Assert.Contains(comments.First().Token, resultString);
    //}
}