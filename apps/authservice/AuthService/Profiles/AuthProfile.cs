using Waggle.AuthService.Dtos;
using Waggle.AuthService.Models;
using AutoMapper;
using Waggle.AuthService.Services;
using Waggle.Common.Helpers;

namespace Waggle.AuthService.Profiles
{
    public class AuthProfile : Profile
    {
        public AuthProfile()
        {
            CreateMap<TokenResponse, TokenResponseDto>();
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
        }
    }
}
