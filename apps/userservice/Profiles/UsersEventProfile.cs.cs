using AutoMapper;
using UserService.Dtos;
using Waggle.Contracts.Auth.Events;

namespace UserService.Profiles
{
    public class UsersEventProfile : Profile
    {
        public UsersEventProfile()
        {
            CreateMap<RegisteredEvent, UserCreateDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.UserId)); ;
        }
    }
}
