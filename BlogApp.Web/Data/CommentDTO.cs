namespace BlogApp.Web.Data;

public class CommentDTO
{
    public required string Text { get; set; }
    public DateTime DatePosted { get; set; }
    public required string Name { get; set; }
}
