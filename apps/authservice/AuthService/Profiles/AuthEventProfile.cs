using Waggle.AuthService.Dtos;
using AutoMapper;
using Waggle.Contracts.Auth.Events;
using Waggle.AuthService.Saga.Context;

namespace Waggle.AuthService.Profiles
{
    public class AuthEventProfile : Profile
    {
        public AuthEventProfile()
        {
            CreateMap<RegisterRequestDto, RegisteredEvent>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<RegistrationSagaContext, RegisteredEvent>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));

            CreateMap<RegistrationSagaContext, RegistrationCompletedEvent>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));

            CreateMap<DeletionSagaContext, UserDeletedEvent>();

            CreateMap<Guid, UserDeletedEvent>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src));
        }
    }
}
