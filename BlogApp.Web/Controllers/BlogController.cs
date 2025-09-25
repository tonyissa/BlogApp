using Microsoft.AspNetCore.Mvc;
using BlogApp.Web.Services;
using BlogApp.Web.Data.DTOs;

namespace BlogApp.Web.Controllers;

public class BlogController(BlogService blogService) : Controller
{
    private readonly BlogService _blogService = blogService;

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
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreatePost([Bind("Title,Body,DatePosted")] PostDTO newPost, string admin_key)
    {
        if (!ModelState.IsValid)
            return View(newPost);

        try
        {
            await _blogService.AddPostAsync(admin_key, newPost);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: Posts/Edit/this-is-a-sample-post
    [Route("posts/edit/{slug}")]
    public async Task<IActionResult> EditPost(string slug)
    {
        var post = await _blogService.GetPostAsync(slug);
        if (post == null)
        {
            return NotFound();
        }

        return View(post);
    }

    // POST: Posts/Edit/5
    [Route("posts/edit/{slug}")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditPost([Bind("Title,Body,DatePosted")] PostDTO editedPost, string slug, string admin_key)
    {
        if (!ModelState.IsValid)
            return View(editedPost);

        try
        {
            await _blogService.EditPostAsync(admin_key, editedPost, slug);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: Posts/Delete/this-is-a-sample-post
    [Route("posts/Delete/{slug}")]
    public async Task<IActionResult> DeletePost(string slug)
    {
        var post = await _blogService.GetPostAsync(slug);
        if (post == null)
        {
            return NotFound();
        }

        return View(post);
    }

    // POST: Posts/Delete/this-is-a-sample-post
    [Route("posts/delete/{slug}")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string slug, string admin_key)
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
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateComment([Bind("Text,DatePosted,Name")] CommentDTO newComment, string slug)
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
    [Route("posts/delete-comment")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteComment(string admin_key, string token)
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
