namespace BlogApp.Web.Data;

public class PostDTO
{
    public required string Title { get; set; }
    public required string Body { get; set; }
    public required DateTime DatePosted { get; set; }
    public required string Slug { get; set; }
}