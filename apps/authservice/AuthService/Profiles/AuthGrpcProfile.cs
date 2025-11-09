using Waggle.AuthService.Dtos;
using AutoMapper;
using Waggle.Contracts.Auth.Grpc;

namespace Waggle.AuthService.Profiles
{
    public class AuthGrpcProfile : Profile
    {
        public AuthGrpcProfile()
        {
            CreateMap<LoginRequest, LoginRequestDto>();
            CreateMap<RegisterRequest, RegisterRequestDto>();
            CreateMap<RefreshTokenRequest, RefreshTokenRequestDto>();
            CreateMap<LogoutRequest, LogoutRequestDto>();
            CreateMap<ValidateTokenRequest, ValidateTokenRequestDto>();
        }
    }
}
