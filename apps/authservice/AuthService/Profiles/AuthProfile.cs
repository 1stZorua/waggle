using AutoMapper;
using Waggle.AuthService.Dtos;
using Waggle.AuthService.Models;
using Waggle.AuthService.Saga.Context;
using Waggle.AuthService.Services;
using Waggle.Common.Helpers;

namespace Waggle.AuthService.Profiles
{
    public class AuthProfile : Profile
    {
        public AuthProfile()
        {
            CreateMap<TokenResponse, TokenResponseDto>();
            CreateMap<RegisterRequestDto, RegisterResponseDto>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<RegisterRequestDto, KeycloakUserRequest>()
                .ConstructUsing((r, ctx) => new(
                    r.Username,
                    r.Email,
                    r.FirstName,
                    r.LastName,
                    r.Password,
                    ctx.GetItem<bool>("Enabled"),
                    ctx.GetItem<Credential[]>("Credentials") ?? []
                ));
            CreateMap<RegistrationSagaContext, KeycloakUserRequest>()
                .ConstructUsing((r, ctx) => new KeycloakUserRequest(
                    r.Username,
                    r.Email,
                    r.FirstName,
                    r.LastName,
                    r.Password,
                    ctx.GetItem<bool>("Enabled"),
                    ctx.GetItem<Credential[]>("Credentials") ?? []
                ));
            CreateMap<RegisterRequestDto, RegistrationSagaContext>();
            CreateMap<RegistrationSagaContext, RegisterResponseDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()));

            CreateMap<DeleteUserRequestDto, DeletionSagaContext>();
        }
    }
}
