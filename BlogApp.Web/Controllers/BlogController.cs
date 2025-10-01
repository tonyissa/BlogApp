using Microsoft.AspNetCore.Mvc;
using BlogApp.Web.Data.DTOs;
using BlogApp.Web.Interfaces;

namespace BlogApp.Web.Controllers;

public class BlogController(IBlogService blogService) : Controller
{
    private readonly IBlogService _blogService = blogService;

    // GET: /
    [Route("")]
    public async Task<IActionResult> Index() => View(await _blogService.GetAllPostsAsync());

    // GET: Posts/this-is-a-sample-post
    [Route("posts/{slug}")]
    public async Task<IActionResult> GetPost(string slug)
    {
        var post = await _blogService.GetPostAsync(slug);
        if (post == null)
        {
            return NotFound();
        }

        return View(post);
    }

    // GET: Posts/Create
    [Route("posts/create")]
    public IActionResult CreatePost()
    {
        return View();
    }

    // POST: Posts/Create
    [Route("posts/create")]
    [HttpPost]
    public async Task<IActionResult> CreatePost([FromBody] PostDTO newPost, [FromHeader] string admin_key)
    {
        if (!ModelState.IsValid)
            return View(newPost);

        try
        {
            await _blogService.AddPostAsync(admin_key, newPost);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: Posts/Delete/this-is-a-sample-post
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

    // POST: Posts/Delete/this-is-a-sample-post
    [Route("posts/{slug}/delete")]
    [HttpPost]
    public async Task<IActionResult> Delete([FromRoute] string slug, [FromHeader] string admin_key)
    {
        try
        {
            await _blogService.DeletePostAsync(admin_key, slug);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

        return RedirectToAction(nameof(Index));
    }

    // POST: Posts/this-is-a-sample-post/comment
    [Route("posts/{slug}/create-comment")]
    [HttpPost]
    public async Task<IActionResult> CreateComment([FromRoute] string slug, [FromBody] CommentDTO newComment)
    {
        try
        {
            await _blogService.AddCommentAsync(newComment, slug);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

        return RedirectToAction(nameof(Index));
    }

    // POST: Posts/comment
    [Route("posts/{slug}/delete-comment")]
    [HttpPost]
    public async Task<IActionResult> DeleteComment([FromHeader] string admin_key, [FromRoute] string token)
    {
        try
        {
            await _blogService.DeleteCommentAsync(admin_key, token);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

        return RedirectToAction(nameof(Index));
    }
}
