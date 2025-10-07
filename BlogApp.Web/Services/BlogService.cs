using BlogApp.Web.Data;
using BlogApp.Web.Extensions;
using BlogApp.Web.Interfaces;
using BlogApp.Web.Models.DTOs;
using BlogApp.Web.Options;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace BlogApp.Web.Services;

public class BlogService(BlogContext context, IOptions<AdminOptions> adminOptions) : IBlogService
{
    private readonly BlogContext _context = context;
    private readonly IOptions<AdminOptions> _adminOptions = adminOptions;

    public async Task<IEnumerable<PostDTO>> GetAllPostsAsync() => 
        await _context.Posts
            .OrderByDescending(p => p.DatePosted)
            .Select(p => p.MapToObject())
            .ToListAsync();

    public async Task<PostDTO?> GetPostAsync(string slug) => 
        await _context.Posts
            .Where(p => p.Slug == slug)
            .Include(p => p.Comments)
            .Select(p => p.MapToObject())
            .FirstOrDefaultAsync();

    public async Task<string> AddPostAsync(string adminKey, PostDTO postDTO)
    {
        if (adminKey != _adminOptions.Value.Key)
            throw new UnauthorizedAccessException("Invalid admin key.");

        var post = postDTO.MapToModel();
        post.Slug = Sluggify(post.Title);

        _context.Posts.Add(post);
        try
        {
            await _context.SaveChangesAsync();
            return post.Slug;
        }
        catch (Exception ex)
        {
            if (ex.InnerException is SqlException sqlEx && sqlEx.Number == 2601)
                throw new InvalidOperationException("A post with the title already exists.");
            throw new DbUpdateException("An error occurred with this request.");
        }
    }

    public async Task DeletePostAsync(string adminKey, string slug)
    {
        if (adminKey != _adminOptions.Value.Key)
            throw new UnauthorizedAccessException("Invalid admin key.");

        var post = await _context.Posts.FirstOrDefaultAsync(p => p.Slug == slug) ?? 
            throw new KeyNotFoundException("Post not found.");

        _context.Posts.Remove(post);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new InvalidOperationException("Concurrency error: Comment was modified or deleted already.");
        }
        catch (Exception)
        {
            throw new DbUpdateException("An error occurred with this request.");
        }
    }

    public async Task AddCommentAsync(CommentDTO commentDTO, string slug)
    {
        var post = await _context.Posts.FirstOrDefaultAsync(p => p.Slug == slug) ?? 
            throw new KeyNotFoundException("Post not found.");

        var comment = commentDTO.MapToModel();
        comment.Token = GenerateToken(commentDTO);
        comment.PostId = post.PostId;
        _context.Comments.Add(comment);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception)
        {
            throw new DbUpdateException("An error occurred with this request.");
        }
    }

    public static string Sluggify(string input) => new([.. input
        .ToLower()
        .Replace("-", "")
        .Replace(' ', '-')
        .Where(c => char.IsLetterOrDigit(c) || c == '-')]);

    public static string GenerateToken(CommentDTO comment)
    {
        var input = $"{comment.Name}{comment.Text}{comment.DatePosted.Ticks}";
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToBase64String(bytes).Replace("/", "_").Replace("+", "-");
    }
}
