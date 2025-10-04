namespace BlogApp.Web.Models.DTOs;

public class CommentDTO
{
    public int PostId { get; set; }
    public string Text { get; set; }
    public DateTime DatePosted { get; set; }
    public string Name { get; set; }
}
