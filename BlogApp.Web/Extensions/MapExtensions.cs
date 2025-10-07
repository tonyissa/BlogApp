using BlogApp.Web.Models;
using BlogApp.Web.Models.DTOs;
using BlogApp.Web.Models.ViewModels;

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
        Name = comment.Name,
        Token = comment.Token
    };

    // Map CommentDTO to Comment
    public static Comment MapToModel(this CommentDTO commentDTO) => new()
    {
        Text = commentDTO.Text,
        DatePosted = commentDTO.DatePosted,
        Name = commentDTO.Name,
        PostId = commentDTO.PostId
    };

    // Map CreatePostViewModel to PostDTO
    public static PostDTO MapToObject(this CreatePostViewModel model) => new()
    {
        Title = model.Title,
        Body = model.Body,
    };

    // Map PostDTO to DeletePostViewModel
    public static DeletePostViewModel MapToDeleteViewModel(this PostDTO postDTO) => new() 
    { 
        Slug = postDTO.Slug,
        Title = postDTO.Title,
    };

    // Map PostDTO to DetailsViewModel
    public static DetailsViewModel MapToDetailsViewModel(this PostDTO postDTO) => new()
    {
        Title = postDTO.Title,
        Body = postDTO.Body,
        Comments = postDTO.Comments,
        Slug = postDTO.Slug
    };

    // Map CreateCommentViewModel to CommentDTO
    public static CommentDTO MapToObject(this CreateCommentViewModel model) => new()
    {
        Text = model.CommentText,
        Name = model.CommentName
    };

    // Map CommentDTO to DeleteCommentViewModel
    public static DeleteCommentViewModel MapToDeleteViewModel(this CommentDTO commentDTO) => new() 
    { 
        Token = commentDTO.Token,
        Author = commentDTO.Name,
        Comment = commentDTO.Text,
        DatePosted = commentDTO.DatePosted,
    };
}