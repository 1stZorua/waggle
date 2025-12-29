using AutoMapper;
using Waggle.CommentService.Dtos;

namespace Waggle.CommentService.Profiles
{
    public class CommentsProfile : Profile
    {
        public CommentsProfile()
        {
            CreateMap<Models.Comment, CommentDto>();
            CreateMap<CommentCreateDto, Models.Comment>();
            CreateMap<Models.Comment, CommentUpdateDto>();
        }
    }
}
