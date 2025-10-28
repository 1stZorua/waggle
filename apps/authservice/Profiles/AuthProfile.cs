using AuthService.Dtos;
using AuthService.Models;
using AutoMapper;

namespace AuthService.Profiles
{
    public class AuthProfile : Profile
    {
        public AuthProfile()
        {
            CreateMap<TokenResponse, TokenResponseDto>();
        }
    }
}
