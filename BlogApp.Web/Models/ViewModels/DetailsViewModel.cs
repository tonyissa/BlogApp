using BlogApp.Web.Models.DTOs;

namespace BlogApp.Web.Models.ViewModels;

public class DetailsViewModel
{
    public string Title { get; set; }
    public string Body { get; set; }
    public string Slug { get; set; }
    public List<CommentDTO> Comments { get; set; } = [];
}