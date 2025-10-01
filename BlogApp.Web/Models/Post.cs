namespace BlogApp.Web.Models;

public class Post
{
    public int PostId { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public List<Comment> Comments { get; set; } = [];
    public DateTime DatePosted { get; set; }
    public string Slug { get; set; }
    public byte[] RowVersion { get; set; }
}
