using AutoMapper;
using Waggle.FollowService.Dtos;

namespace Waggle.FollowService.Profiles
{
    public class FollowsProfile : Profile
    {
        public FollowsProfile()
        {
            CreateMap<Models.Follow, FollowDto>();
            CreateMap<FollowCreateDto, Models.Follow>();
        }
    }
}
