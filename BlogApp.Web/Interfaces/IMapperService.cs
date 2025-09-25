using BlogApp.Web.Data.DTOs;
using BlogApp.Web.Models;

namespace BlogApp.Web.Interfaces;

public interface IMapperService
{
    PostDTO MapToDTO(Post post);
    Post MapToModel(PostDTO postDTO, Post? post = null);
    CommentDTO MapToDTO(Comment comment);
    Comment MapToModel(CommentDTO commentDTO);
}