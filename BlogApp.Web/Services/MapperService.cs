using AutoMapper;
using BlogApp.Web.Data.DTOs;
using BlogApp.Web.Interfaces;
using BlogApp.Web.Models;

namespace BlogApp.Web.Services;

public class MapperService(IMapper mapper) : IMapperService
{
    private readonly IMapper _mapper = mapper;

    public PostDTO MapToDTO(Post post) => _mapper.Map<PostDTO>(post);
    public Post MapToModel(PostDTO postDTO, Post? post = null) => post != null 
        ? _mapper.Map(postDTO, post)
        : _mapper.Map<Post>(postDTO);
    public CommentDTO MapToDTO(Comment comment) => _mapper.Map<CommentDTO>(comment);
    public Comment MapToModel(CommentDTO commentDTO) => _mapper.Map<Comment>(commentDTO);
}
