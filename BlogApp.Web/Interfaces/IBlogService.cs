using BlogApp.Web.Models.DTOs;

namespace BlogApp.Web.Interfaces;

public interface IBlogService
{
    Task<IEnumerable<PostDTO>> GetAllPostsAsync();
    Task<PostDTO?> GetPostAsync(string slug);
    Task<string> AddPostAsync(string adminKey, PostDTO postDTO);
    Task DeletePostAsync(string adminKey, string slug);
    Task AddCommentAsync(CommentDTO commentDTO, string slug);
    Task DeleteCommentAsync(string adminKey, string commentToken);
}