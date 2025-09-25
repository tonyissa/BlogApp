namespace BlogApp.Web.Models;

public class Comment
{
    public int CommentId { get; set; }
    public required int PostId { get; set; }
    public Post? Post { get; set; }
    public required string Text { get; set; }
    public required DateTime DatePosted { get; set; }
    public required string Name { get; set; }
}
