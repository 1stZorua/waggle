using Waggle.AuthService.Dtos;
using AutoMapper;
using Waggle.Contracts.Auth.Events;

namespace Waggle.AuthService.Profiles
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
