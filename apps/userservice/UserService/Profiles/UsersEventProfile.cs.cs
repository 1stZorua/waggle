using AutoMapper;
using Waggle.Contracts.Auth.Events;
using Waggle.UserService.Dtos;

namespace Waggle.UserService.Profiles
{
    public class UsersEventProfile : Profile
    {
        public UsersEventProfile()
        {
            CreateMap<RegisteredEvent, UserCreateDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id)); ;
        }
    }
}
