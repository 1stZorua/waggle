using AuthService.Dtos;
using AuthService.Grpc;
using AuthService.Models;
using AutoMapper;

namespace AuthService.Profiles
{
    public class AuthProfile : Profile
    {
        public AuthProfile()
        {
            CreateMap<TokenResponse, TokenResponseDto>();

            CreateMap<LoginRequest, LoginRequestDto>();
            CreateMap<RegisterRequest, RegisterRequestDto>();
            CreateMap<RefreshTokenRequest, RefreshTokenRequestDto>();
            CreateMap<LogoutRequest, LogoutRequestDto>();
            CreateMap<ValidateTokenRequest, ValidateTokenRequestDto>();
        }
    }
}
