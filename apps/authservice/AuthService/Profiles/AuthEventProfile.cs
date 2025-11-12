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
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<Guid, DeletedEvent>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src));
        }
    }
}
