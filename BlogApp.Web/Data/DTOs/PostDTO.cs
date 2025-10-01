namespace BlogApp.Web.Data.DTOs;

public class PostDTO
{
    public string Title { get; set; }
    public string Body { get; set; }
    public DateTime DatePosted { get; set; }
    public string Slug { get; set; }
    public List<CommentDTO> Comments { get; set; } = [];
}