using AutoMapper;
using Waggle.Contracts.Like.Events;
using Waggle.LikeService.Models;

namespace Waggle.LikeService.Profiles
{
    public class LikesEventProfile : Profile
    {
        public LikesEventProfile()
        {
            CreateMap<Like, LikeCreatedEvent>();
            CreateMap<Like, LikeDeletedEvent>();
        }
    }
}
