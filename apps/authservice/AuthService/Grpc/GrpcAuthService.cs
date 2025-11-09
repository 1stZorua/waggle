using Waggle.AuthService.Constants;
using Waggle.AuthService.Dtos;
using Waggle.AuthService.Services;
using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Waggle.Common.Grpc;
using Waggle.Contracts.Auth.Grpc;

namespace Waggle.AuthService.Grpc
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
                throw GrpcExceptionHelper.CreateRpcException(result.Message ?? AuthErrors.Token.RetrievalFailed, result.ErrorCode);

            return _mapper.Map<LoginResponse>(result.Data);
        }

        public override async Task<RegisterResponse> Register(RegisterRequest request, ServerCallContext context)
        {
            var dto = _mapper.Map<RegisterRequestDto>(request);
            var result = await _service.CreateUserAsync(dto);

            if (!result.Success)
                throw GrpcExceptionHelper.CreateRpcException(result.Message ?? AuthErrors.User.CreationFailed, result.ErrorCode);

            return _mapper.Map<RegisterResponse>(result.Data);
        }

        public override async Task<RefreshTokenResponse> RefreshToken(RefreshTokenRequest request, ServerCallContext context)
        {
            var dto = _mapper.Map<RefreshTokenRequestDto>(request);
            var result = await _service.RefreshTokenAsync(dto);

            if (!result.Success)
                throw GrpcExceptionHelper.CreateRpcException(result.Message ?? AuthErrors.Token.RetrievalFailed, result.ErrorCode);

            return _mapper.Map<RefreshTokenResponse>(result.Data);
        }

        public override async Task<Empty> Logout(LogoutRequest request, ServerCallContext context)
        {
            var dto = _mapper.Map<LogoutRequestDto>(request);
            var result = await _service.LogoutAsync(dto);

            if (!result.Success)
                throw GrpcExceptionHelper.CreateRpcException(result.Message ?? AuthErrors.Session.EndFailed, result.ErrorCode);

            return new Empty();
        }

        public override async Task<ValidateTokenResponse> Validate(ValidateTokenRequest request, ServerCallContext context)
        {
            var dto = _mapper.Map<ValidateTokenRequestDto>(request);
            var result = await _service.ValidateAsync(dto);

            if (!result.Success)
                throw GrpcExceptionHelper.CreateRpcException(result.Message ?? AuthErrors.Token.InvalidFormat, result.ErrorCode);

            return _mapper.Map<ValidateTokenResponse>(result.Data);
        }
    }
}