namespace BlogApp.Web.Models;

public class Comment
{
    public int CommentId { get; set; }
    public int PostId { get; set; }
    public Post Post { get; set; }
    public string Text { get; set; }
    public DateTime DatePosted { get; set; }
    public string Name { get; set; }
    public string Token { get; set; }
    public byte[] RowVersion { get; set; }
}