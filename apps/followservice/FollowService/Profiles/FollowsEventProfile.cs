using AutoMapper;
using Waggle.Contracts.Follow.Events;
using Waggle.FollowService.Models;

namespace Waggle.FollowService.Profiles
{
    public class FollowsEventProfile : Profile
    {
        public FollowsEventProfile()
        {
            CreateMap<Follow, FollowCreatedEvent>();
            CreateMap<Follow, FollowDeletedEvent>();
        }
    }
}
