using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Waggle.Common.Constants;
using Waggle.Common.Grpc;
using Waggle.Contracts.User.Grpc;
using Waggle.UserService.Constants;
using Waggle.UserService.Dtos;
using Waggle.UserService.Services;

namespace Waggle.UserService.Grpc
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

        public override async Task<GetUsersResponse> GetUsers(GetUsersRequest request, ServerCallContext context)
        {
            var paginationRequest = new Common.Pagination.Models.PaginationRequest
            {
                Cursor = request.Cursor,
                PageSize = request.PageSize,
                Direction = request.Direction == PaginationDirection.Backward
                    ? Common.Pagination.Models.PaginationDirection.Backward
                    : Common.Pagination.Models.PaginationDirection.Forward
            };

            var result = await _service.GetUsersAsync(paginationRequest);

            if (!result.Success)
                throw GrpcExceptionHelper.CreateRpcException(result.Message, result.ErrorCode);

            var response = new GetUsersResponse();
            response.Users.AddRange(_mapper.Map<IEnumerable<User>>(result.Data!.Items));
            response.PageInfo = _mapper.Map<PageInfo>(result.Data.PageInfo);

            return response;
        }

        public override async Task<GetUserByIdResponse> GetUserById(GetUserByIdRequest request, ServerCallContext context)
        {
            if (!Guid.TryParse(request.Id, out var userId))
                throw GrpcExceptionHelper.CreateRpcException(ErrorCodes.InvalidInput, UserErrors.User.InvalidId);

            var result = await _service.GetUserByIdAsync(userId);
            if (!result.Success)
                throw GrpcExceptionHelper.CreateRpcException(result.Message, result.ErrorCode);

            return _mapper.Map<GetUserByIdResponse>(result.Data);
        }

        public override async Task<GetUsersByIdResponse> GetUsersById(GetUsersByIdRequest request, ServerCallContext context)
        {
            var batchRequest = _mapper.Map<UserBatchRequest>(request);
            var result = await _service.GetUsersByIdAsync(batchRequest);

            if (!result.Success)
                throw GrpcExceptionHelper.CreateRpcException(result.Message, result.ErrorCode);

            return _mapper.Map<GetUsersByIdResponse>(result.Data);
        }

        public override async Task<CreateUserResponse> CreateUser(CreateUserRequest request, ServerCallContext context)
        {
            var dto = _mapper.Map<UserCreateDto>(request);
            var result = await _service.CreateUserAsync(dto);

            if (!result.Success)
                throw GrpcExceptionHelper.CreateRpcException(result.Message, result.ErrorCode);
            
            return _mapper.Map<CreateUserResponse>(result.Data);
        }

        public override async Task<Empty> DeleteUser(DeleteUserRequest request, ServerCallContext context)
        {
            if (!Guid.TryParse(request.Id, out var userId))
                throw GrpcExceptionHelper.CreateRpcException(ErrorCodes.InvalidInput, UserErrors.User.InvalidId);

            var result = await _service.DeleteUserAsync(userId);
            if (!result.Success)
                throw GrpcExceptionHelper.CreateRpcException(result.Message, result.ErrorCode);

            return new Empty();
        }
    }
}
