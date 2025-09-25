using BlogApp.Web.Data.DTOs;

namespace BlogApp.Web.Interfaces;

public interface IBlogService
{
    Task<IEnumerable<PostDTO>> GetAllPostsAsync();
    Task<PostDTO?> GetPostAsync(string slug);
    Task AddPostAsync(string adminKey, PostDTO postDTO);
    Task EditPostAsync(string adminKey, PostDTO postDTO, string originalSlug);
    Task DeletePostAsync(string adminKey, string slug);
    Task AddCommentAsync(CommentDTO commentDTO, string slug);
    Task DeleteCommentAsync(string adminKey, string commentToken);
}