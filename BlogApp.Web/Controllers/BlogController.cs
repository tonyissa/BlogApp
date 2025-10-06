using BlogApp.Web.Extensions;
using BlogApp.Web.Interfaces;
using BlogApp.Web.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Web.Controllers;

public class BlogController(IBlogService blogService) : Controller
{
    private readonly IBlogService _blogService = blogService;

    // GET: /
    [HttpGet("")]
    public async Task<IActionResult> Index() => View(await _blogService.GetAllPostsAsync());

    // GET: Posts/this-is-a-sample-post
    [HttpGet("posts/{slug}")]
    public async Task<IActionResult> Details(string slug)
    {
        var post = await _blogService.GetPostAsync(slug);
        if (post == null)
        {
            return NotFound();
        }

        return View(post);
    }

    // GET: Posts/Create
    [HttpGet("posts/create")]
    public IActionResult CreatePost() => View();

    // POST: Posts/Create
    [HttpPost("posts/create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreatePost([Bind("Title,Body,AdminKey")] CreatePostViewModel createPostRequest)
    {
        if (!ModelState.IsValid)
            return View(createPostRequest);

        string slug;
        try
        {
            slug = await _blogService.AddPostAsync(createPostRequest.AdminKey, createPostRequest.MapToObject());
        }
        catch (UnauthorizedAccessException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(createPostRequest);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
            return View(createPostRequest);
        }

        return RedirectToAction(nameof(Details), slug);
    }

    // GET: Posts/this-is-a-sample-post/Delete
    [Route("posts/{slug}/delete")]
    public async Task<IActionResult> DeletePost([FromRoute] string slug)
    {
        var post = await _blogService.GetPostAsync(slug);
        if (post == null)
        {
            return NotFound();
        }

        return View(post);
    }

    // POST: Posts/this-is-a-sample-post/Delete
    [HttpPost("posts/{slug}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeletePost([Bind("AdminKey")] DeletePostViewModel deletePostRequest, string slug)
    {
        try
        {
            await _blogService.DeletePostAsync(deletePostRequest.AdminKey, slug);
        }
        catch (UnauthorizedAccessException ex)
        {
            ModelState.AddModelError(nameof(deletePostRequest.AdminKey), ex.Message);
            return View(deletePostRequest);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
            return View(deletePostRequest);
        }

        return RedirectToAction(nameof(Index));
    }

    // POST: Posts/this-is-a-sample-post/Comment
    [HttpPost("posts/{slug}/comment")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateComment([Bind("Text,Name")] CreateCommentViewModel newComment, string slug)
    {
        if (!ModelState.IsValid)
            return View(newComment);

        try
        {
            await _blogService.AddCommentAsync(newComment.MapToObject(), slug);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
            return View(newComment);
        }

        return RedirectToAction(nameof(Details), slug);
    }
}
