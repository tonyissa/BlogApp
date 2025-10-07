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
            return NotFound();

        return View(post.MapToDetailsViewModel());
    }

    // GET: Posts/Create
    [HttpGet("posts/create")]
    public IActionResult CreatePost() => View();

    // POST: Posts/Create
    [HttpPost("posts/create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreatePost([Bind("Title,Body,AdminKey")] CreatePostViewModel createPostVM)
    {
        if (!ModelState.IsValid)
            return View(createPostVM);

        string slug;
        try
        {
            slug = await _blogService.AddPostAsync(createPostVM.AdminKey, createPostVM.MapToObject());
        }
        catch (UnauthorizedAccessException ex)
        {
            ModelState.AddModelError(nameof(createPostVM.AdminKey), ex.Message);
            return View(createPostVM);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
            return View(createPostVM);
        }

        return RedirectToAction(nameof(Details), new { slug });
    }

    // GET: Posts/this-is-a-sample-post/Delete
    [Route("posts/{slug}/delete")]
    public async Task<IActionResult> DeletePost([FromRoute] string slug)
    {
        var post = await _blogService.GetPostAsync(slug);
        if (post == null)
            return NotFound();

        return View(post.MapToDeleteViewModel());
    }

    // POST: Posts/this-is-a-sample-post/Delete
    [HttpPost("posts/{slug}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeletePost(
        [Bind("Title,Slug,AdminKey")] DeletePostViewModel deletePostVM, 
        string slug)
    {
        if (!ModelState.IsValid)
            return View(deletePostVM);

        try
        {
            await _blogService.DeletePostAsync(deletePostVM.AdminKey, slug);
        }
        catch (UnauthorizedAccessException ex)
        {
            ModelState.AddModelError(nameof(deletePostVM.AdminKey), ex.Message);
            return View(deletePostVM);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
            return View(deletePostVM);
        }

        return RedirectToAction(nameof(Index));
    }

    // POST: Posts/this-is-a-sample-post/Comment
    [HttpPost("posts/{slug}/comment")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateComment(
        [Bind("CommentText,CommentName,Slug")] CreateCommentViewModel createCommentVM, 
        string slug)
    {
        var post = await _blogService.GetPostAsync(slug);
        if (post == null)
            return NotFound();

        var detailsModel = post.MapToDetailsViewModel();
        ViewData["CommentModel"] = createCommentVM;

        if (!ModelState.IsValid)
            return View("Details", detailsModel);

        try
        {
            await _blogService.AddCommentAsync(createCommentVM.MapToObject(), createCommentVM.Slug);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
            return View("Details", detailsModel);
        }

        return RedirectToAction(nameof(Details), new { slug });
    }

    // GET: Posts/this-is-a-sample-post/delete-comment/da2dera4va2dad-a2d2dad2-d2d1
    [HttpGet("posts/{slug}/delete-comment/{token}")]
    public async Task<IActionResult> DeleteComment(string token)
    {
        var comment = await _blogService.GetCommentAsync(token);
        if (comment == null)
            return NotFound();

        return View(comment.MapToDeleteViewModel());
    }

    // POST: Posts/this-is-a-sample-post/delete-comment/da2dera4va2dad-a2d2dad2-d2d1
    [HttpPost("posts/{slug}/delete-comment/{token}")]
    public async Task<IActionResult> DeleteComment([Bind("AdminKey")] DeleteCommentViewModel deleteCommentVM, string slug)
    {
        if (!ModelState.IsValid)
            return View(deleteCommentVM);

        try
        {
            await _blogService.DeleteCommentAsync(deleteCommentVM.AdminKey, deleteCommentVM.Token);
        }
        catch (UnauthorizedAccessException ex)
        {
            ModelState.AddModelError(nameof(deleteCommentVM.AdminKey), ex.Message);
            return View(deleteCommentVM);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
            return View(deleteCommentVM);
        }

        return RedirectToAction(nameof(Details), new { slug });
    }
}