using AutoMapper;
using Waggle.LikeService.Dtos;

namespace Waggle.LikeService.Profiles
{
    public class LikesProfile : Profile
    {
        public LikesProfile()
        {
            CreateMap<Models.Like, LikeDto>();
            CreateMap<LikeCreateDto, Models.Like>();
        }
    }
}
