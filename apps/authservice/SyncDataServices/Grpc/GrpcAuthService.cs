using AuthService.Dtos;
using AuthService.Grpc;
using AuthService.Services;
using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Waggle.Common.Grpc;

namespace AuthService.SyncDataServices.Grpc
{
    public class GrpcAuthService : GrpcAuth.GrpcAuthBase
    {
        private readonly IAuthService _service;
        private readonly IMapper _mapper;

        public GrpcAuthService(IAuthService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        public override async Task<LoginResponse> Login(LoginRequest request, ServerCallContext context)
        {
            var dto = _mapper.Map<LoginRequestDto>(request);
            var result = await _service.PasswordGrantAsync(dto);

            if (!result.Success)
                throw GrpcExceptionHelper.CreateRpcException(result.ErrorCode, result.Message ?? "Login failed");

            return _mapper.Map<LoginResponse>(result.Data);
        }

        public override async Task<RegisterResponse> Register(RegisterRequest request, ServerCallContext context)
        {
            var dto = _mapper.Map<RegisterRequestDto>(request);
            var result = await _service.CreateUserAsync(dto);

            if (!result.Success)
                throw GrpcExceptionHelper.CreateRpcException(result.ErrorCode, result.Message ?? "Registration failed");

            return _mapper.Map<RegisterResponse>(result.Data);
        }

        public override async Task<RefreshTokenResponse> RefreshToken(RefreshTokenRequest request, ServerCallContext context)
        {
            var dto = _mapper.Map<RefreshTokenRequestDto>(request);
            var result = await _service.RefreshTokenAsync(dto);

            if (!result.Success)
                throw GrpcExceptionHelper.CreateRpcException(result.ErrorCode, result.Message ?? "Token refresh failed");

            return _mapper.Map<RefreshTokenResponse>(result.Data);
        }

        public override async Task<Empty> Logout(LogoutRequest request, ServerCallContext context)
        {
            var dto = _mapper.Map<LogoutRequestDto>(request);
            var result = await _service.LogoutAsync(dto);

            if (!result.Success)
                throw GrpcExceptionHelper.CreateRpcException(result.ErrorCode, result.Message ?? "Logout failed");

            return new Empty();
        }

        public override async Task<ValidateTokenResponse> Validate(ValidateTokenRequest request, ServerCallContext context)
        {
            var dto = _mapper.Map<ValidateTokenRequestDto>(request);
            var result = await _service.ValidateAsync(dto);

            if (!result.Success)
                throw GrpcExceptionHelper.CreateRpcException(result.ErrorCode, result.Message ?? "Token validation failed");

            return _mapper.Map<ValidateTokenResponse>(result.Data);
        }
    }
}