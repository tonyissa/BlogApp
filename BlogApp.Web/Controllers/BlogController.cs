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

        return View(post.MapToDetailsViewModel());
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
            ModelState.AddModelError(nameof(createPostRequest.AdminKey), ex.Message);
            return View(createPostRequest);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
            return View(createPostRequest);
        }

        return RedirectToAction(nameof(Details), new { slug });
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

        return View(post.MapToDeleteViewModel());
    }

    // POST: Posts/Delete
    [HttpPost("posts/{slug}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeletePost([Bind("Title,Slug,AdminKey")] DeletePostViewModel deletePostRequest)
    {
        if (!ModelState.IsValid)
            return View(deletePostRequest);

        try
        {
            await _blogService.DeletePostAsync(deletePostRequest.AdminKey, deletePostRequest.Slug);
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
    public async Task<IActionResult> CreateComment([Bind("CommentText,CommentName,Slug")] CreateCommentViewModel newComment, string slug)
    {
        var post = await _blogService.GetPostAsync(slug);
        if (post == null)
        {
            return NotFound();
        }

        var detailsModel = post.MapToDetailsViewModel();
        ViewData["CommentModel"] = newComment;

        if (!ModelState.IsValid)
            return View("Details", detailsModel);

        try
        {
            await _blogService.AddCommentAsync(newComment.MapToObject(), newComment.Slug);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
            return View("Details", detailsModel);
        }

        return RedirectToAction(nameof(Details), new { slug });
    }
}
