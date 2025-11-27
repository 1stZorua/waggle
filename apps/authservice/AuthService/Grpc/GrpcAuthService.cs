using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Waggle.AuthService.Constants;
using Waggle.AuthService.Dtos;
using Waggle.AuthService.Services;
using Waggle.Common.Auth;
using Waggle.Common.Constants;
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
                throw GrpcExceptionHelper.CreateRpcException(result.Message, result.ErrorCode);

            return _mapper.Map<LoginResponse>(result.Data);
        }

        public override async Task<RegisterResponse> Register(RegisterRequest request, ServerCallContext context)
        {
            var dto = _mapper.Map<RegisterRequestDto>(request);
            var result = await _service.CreateUserAsync(dto);

            if (!result.Success)
                throw GrpcExceptionHelper.CreateRpcException(result.Message, result.ErrorCode);

            return _mapper.Map<RegisterResponse>(result.Data);
        }

        public override async Task<RefreshTokenResponse> RefreshToken(RefreshTokenRequest request, ServerCallContext context)
        {
            var dto = _mapper.Map<RefreshTokenRequestDto>(request);
            var result = await _service.RefreshTokenAsync(dto);

            if (!result.Success)
                throw GrpcExceptionHelper.CreateRpcException(result.Message, result.ErrorCode);

            return _mapper.Map<RefreshTokenResponse>(result.Data);
        }

        public override async Task<Empty> Logout(LogoutRequest request, ServerCallContext context)
        {
            var dto = _mapper.Map<LogoutRequestDto>(request);
            var result = await _service.LogoutAsync(dto);

            if (!result.Success)
                throw GrpcExceptionHelper.CreateRpcException(result.Message, result.ErrorCode);

            return new Empty();
        }

        public override async Task<ValidateTokenResponse> Validate(ValidateTokenRequest request, ServerCallContext context)
        {
            var dto = _mapper.Map<ValidateTokenRequestDto>(request);
            var result = await _service.ValidateAsync(dto);

            if (!result.Success)
                throw GrpcExceptionHelper.CreateRpcException(result.Message, result.ErrorCode);

            return _mapper.Map<ValidateTokenResponse>(result.Data);
        }

        public override async Task<Empty> DeleteUser(DeleteUserRequest request, ServerCallContext context)
        {
            if (!Guid.TryParse(request.UserId, out var _))
                throw GrpcExceptionHelper.CreateRpcException(AuthErrors.User.InvalidId, ErrorCodes.InvalidInput);

            var currentUser = await GetCurrentUserAsync(context);

            var dto = _mapper.Map<DeleteUserRequestDto>(request);
            var deleteResult = await _service.DeleteUserAsync(dto, currentUser);

            if (!deleteResult.Success)
                throw GrpcExceptionHelper.CreateRpcException(deleteResult.Message, deleteResult.ErrorCode);

            return new Empty();
        }

        private async Task<UserInfoDto> GetCurrentUserAsync(ServerCallContext context)
        {
            var authHeader = context.RequestHeaders.GetValue("Authorization");
            if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
                throw GrpcExceptionHelper.CreateRpcException(AuthErrors.Token.Missing, ErrorCodes.Unauthorized);

            var token = authHeader["Bearer ".Length..].Trim();

            var result = await _service.ValidateAsync(new ValidateTokenRequestDto { BearerToken = $"Bearer {token}" });
            if (!result.Success || result.Data == null)
                throw GrpcExceptionHelper.CreateRpcException(result.Message, result.ErrorCode);

            return result.Data;
        }
    }
}