namespace BlogApp.Web.Data;

public class PostDTO
{
    public required string Title { get; set; }
    public required string Body { get; set; }
    public DateTime DatePosted { get; set; }
    public string Slug { get; set; }
}