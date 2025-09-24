namespace BlogApp.Web.Models;

public class Post
{
    public int PostId { get; set; }
    public required string Title { get; set; }
    public required string Body { get; set; }
    public List<Comment>? Comments { get; set; }
    public required DateTime DatePosted { get; set; }
}
