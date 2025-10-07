namespace BlogApp.Web.Models.ViewModels;

public class DeleteCommentViewModel
{
    public string Token { get; set; }
    public string Author { get; set; }
    public string Comment { get; set; }
    public string AdminKey { get; set; }
    public DateTime DatePosted { get; set; }
}