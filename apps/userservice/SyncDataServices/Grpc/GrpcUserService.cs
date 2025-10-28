using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using UserService.Constants;
using UserService.Dtos;
using UserService.Services;
using Waggle.Common.Constants;
using Waggle.Common.Grpc;
using Waggle.Contracts.User.Grpc;

namespace UserService.SyncDataServices.Grpc
{
    public class GrpcUserService : GrpcUser.GrpcUserBase
    {
        private readonly IUserService _service;
        private readonly IMapper _mapper;

        public GrpcUserService(IUserService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        public override async Task<GetAllUsersResponse> GetAllUsers(Empty request, ServerCallContext context)
        {
            var result = await _service.GetAllUsersAsync();

            if (!result.Success)
                throw GrpcExceptionHelper.CreateRpcException(result.Message ?? UserErrors.Service.Failed, result.ErrorCode);

            return new GetAllUsersResponse
            {
                Users = { _mapper.Map<IEnumerable<User>>(result.Data) }
            };
        }

        public override async Task<GetUserByIdResponse> GetUserById(GetUserByIdRequest request, ServerCallContext context)
        {
            if (!Guid.TryParse(request.Id, out var userId))
                throw GrpcExceptionHelper.CreateRpcException(ErrorCodes.InvalidInput, UserErrors.User.InvalidId);

            var result = await _service.GetUserByIdAsync(userId);
            if (!result.Success)
                throw GrpcExceptionHelper.CreateRpcException(result.Message ?? UserErrors.User.RetrievalFailed, result.ErrorCode);

            return new GetUserByIdResponse
            {
                User = _mapper.Map<User>(result.Data)
            };
        }

        public override async Task<CreateUserResponse> CreateUser(CreateUserRequest request, ServerCallContext context)
        {
            var dto = _mapper.Map<UserCreateDto>(request);
            var result = await _service.CreateUserAsync(dto);

            if (!result.Success)
                throw GrpcExceptionHelper.CreateRpcException(result.Message ?? UserErrors.User.CreationFailed, result.ErrorCode);

            return new CreateUserResponse
            {
                User = _mapper.Map<User>(result.Data)
            };
        }
    }
}
