using AuthService.Dtos;
using AutoMapper;
using Waggle.Contracts.Auth.Events;

namespace AuthService.Profiles
{
    public class AuthEventProfile : Profile
    {
        public AuthEventProfile()
        {
            CreateMap<RegisterRequestDto, RegisteredEvent>()
                .ForMember(dest => dest.UserId, opt => opt.Ignore());
        }
    }
}
