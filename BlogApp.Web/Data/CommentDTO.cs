namespace BlogApp.Web.Data;

public class CommentDTO
{
    public required string Text { get; set; }
    public required DateTime DatePosted { get; set; }
    public required string Name { get; set; }
}
