using BlogApp.Web.Data.DTOs;
using BlogApp.Web.Models;

namespace BlogApp.Web.Extensions;

public static class MapExtensions
{
    // Map Post to PostDTO
    public static PostDTO MapToObject(this Post post) => new()
    {
        Title = post.Title,
        Body = post.Body,
        DatePosted = post.DatePosted,
        Slug = post.Slug,
        Comments = [.. post.Comments.Select(c => c.MapToObject())]
    };

    // Map PostDTO to Post
    public static Post MapToModel(this PostDTO postDTO) => new()
    {
        Title = postDTO.Title,
        Body = postDTO.Body,
        DatePosted = postDTO.DatePosted,
        Slug = postDTO.Slug
    };

    // Map Comment to CommentDTO
    public static CommentDTO MapToObject(this Comment comment) => new()
    {
        Text = comment.Text,
        DatePosted = comment.DatePosted,
        Name = comment.Name
    };

    // Map CommentDTO to Comment
    public static Comment MapToModel(this CommentDTO commentDTO) => new()
    {
        Text = commentDTO.Text,
        DatePosted = commentDTO.DatePosted,
        Name = commentDTO.Name,
        PostId = commentDTO.PostId
    };
}