using AutoMapper;
using Waggle.PostService.Dtos;

namespace Waggle.PostService.Profiles
{
    public class PostsProfile : Profile
    {
        public PostsProfile()
        {
            CreateMap<Models.Post, PostDto>();
            CreateMap<PostCreateDto, Models.Post>();
        }
    }
}
