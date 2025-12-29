using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Waggle.Common.Auth;
using Waggle.Common.Constants;
using Waggle.Common.Grpc;
using Waggle.Contracts.Follow.Grpc;
using Waggle.FollowService.Constants;
using Waggle.FollowService.Dtos;
using Waggle.FollowService.Services;

namespace Waggle.FollowService.Grpc
{
    public class GrpcFollowService : GrpcFollow.GrpcFollowBase
    {
        private readonly IFollowService _service;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public GrpcFollowService(IFollowService service, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _service = service;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public override async Task<GetFollowsResponse> GetFollows(GetFollowsRequest request, ServerCallContext context)
        {
            var paginationRequest = new Common.Pagination.Models.PaginationRequest
            {
                Cursor = request.Cursor,
                PageSize = request.PageSize,
                Direction = request.Direction == PaginationDirection.Backward
                    ? Common.Pagination.Models.PaginationDirection.Backward
                    : Common.Pagination.Models.PaginationDirection.Forward
            };

            var result = await _service.GetFollowsAsync(paginationRequest);

            if (!result.Success)
                throw GrpcExceptionHelper.CreateRpcException(result.Message, result.ErrorCode);

            var response = new GetFollowsResponse();
            response.Follows.AddRange(_mapper.Map<IEnumerable<Follow>>(result.Data!.Items));
            response.PageInfo = _mapper.Map<PageInfo>(result.Data.PageInfo);

            return response;
        }

        public override async Task<GetFollowByIdResponse> GetFollowById(GetFollowByIdRequest request, ServerCallContext context)
        {
            if (!Guid.TryParse(request.Id, out var followId))
                throw GrpcExceptionHelper.CreateRpcException(ErrorCodes.InvalidInput, FollowErrors.Follow.InvalidId);

            var result = await _service.GetFollowByIdAsync(followId);
            if (!result.Success)
                throw GrpcExceptionHelper.CreateRpcException(result.Message, result.ErrorCode);

            return _mapper.Map<GetFollowByIdResponse>(result.Data);
        }

        public override async Task<GetFollowersResponse> GetFollowers(GetFollowersRequest request, ServerCallContext context)
        {
            if (!Guid.TryParse(request.UserId, out var userId))
                throw GrpcExceptionHelper.CreateRpcException(ErrorCodes.InvalidInput, FollowErrors.Follow.InvalidId);

            var paginationRequest = new Common.Pagination.Models.PaginationRequest
            {
                Cursor = request.Cursor,
                PageSize = request.PageSize,
                Direction = request.Direction == PaginationDirection.Backward
                    ? Common.Pagination.Models.PaginationDirection.Backward
                    : Common.Pagination.Models.PaginationDirection.Forward
            };

            var result = await _service.GetFollowersAsync(userId, paginationRequest);

            if (!result.Success)
                throw GrpcExceptionHelper.CreateRpcException(result.Message, result.ErrorCode);

            var response = new GetFollowersResponse();
            response.Follows.AddRange(_mapper.Map<IEnumerable<Follow>>(result.Data!.Items));
            response.PageInfo = _mapper.Map<PageInfo>(result.Data.PageInfo);

            return response;
        }

        public override async Task<GetFollowingResponse> GetFollowing(GetFollowingRequest request, ServerCallContext context)
        {
            if (!Guid.TryParse(request.UserId, out var userId))
                throw GrpcExceptionHelper.CreateRpcException(ErrorCodes.InvalidInput, FollowErrors.Follow.InvalidId);

            var paginationRequest = new Common.Pagination.Models.PaginationRequest
            {
                Cursor = request.Cursor,
                PageSize = request.PageSize,
                Direction = request.Direction == PaginationDirection.Backward
                    ? Common.Pagination.Models.PaginationDirection.Backward
                    : Common.Pagination.Models.PaginationDirection.Forward
            };

            var result = await _service.GetFollowingAsync(userId, paginationRequest);

            if (!result.Success)
                throw GrpcExceptionHelper.CreateRpcException(result.Message, result.ErrorCode);

            var response = new GetFollowingResponse();
            response.Follows.AddRange(_mapper.Map<IEnumerable<Follow>>(result.Data!.Items));
            response.PageInfo = _mapper.Map<PageInfo>(result.Data.PageInfo);

            return response;
        }

        public override async Task<GetFollowerCountResponse> GetFollowerCount(GetFollowerCountRequest request, ServerCallContext context)
        {
            if (!Guid.TryParse(request.UserId, out var userId))
                throw GrpcExceptionHelper.CreateRpcException(ErrorCodes.InvalidInput, FollowErrors.Follow.InvalidId);

            var result = await _service.GetFollowerCountAsync(userId);

            if (!result.Success)
                throw GrpcExceptionHelper.CreateRpcException(result.Message, result.ErrorCode);

            return new GetFollowerCountResponse
            {
                Count = result.Data
            };
        }

        public override async Task<GetFollowingCountResponse> GetFollowingCount(GetFollowingCountRequest request, ServerCallContext context)
        {
            if (!Guid.TryParse(request.UserId, out var userId))
                throw GrpcExceptionHelper.CreateRpcException(ErrorCodes.InvalidInput, FollowErrors.Follow.InvalidId);

            var result = await _service.GetFollowingCountAsync(userId);

            if (!result.Success)
                throw GrpcExceptionHelper.CreateRpcException(result.Message, result.ErrorCode);

            return new GetFollowingCountResponse
            {
                Count = result.Data
            };
        }

        public override async Task<IsFollowingResponse> IsFollowing(IsFollowingRequest request, ServerCallContext context)
        {
            if (!Guid.TryParse(request.FollowerId, out var followerId))
                throw GrpcExceptionHelper.CreateRpcException(ErrorCodes.InvalidInput, FollowErrors.Follow.InvalidId);

            if (!Guid.TryParse(request.FollowingId, out var followingId))
                throw GrpcExceptionHelper.CreateRpcException(ErrorCodes.InvalidInput, FollowErrors.Follow.InvalidId);

            var result = await _service.IsFollowingAsync(followerId, followingId);

            if (!result.Success)
                throw GrpcExceptionHelper.CreateRpcException(result.Message, result.ErrorCode);

            return _mapper.Map<IsFollowingResponse>(result.Data);
        }

        public override async Task<CreateFollowResponse> CreateFollow(CreateFollowRequest request, ServerCallContext context)
        {
            var dto = _mapper.Map<FollowCreateDto>(request);
            var currentUser = GetCurrentUser();
            var result = await _service.CreateFollowAsync(dto, currentUser);

            if (!result.Success)
                throw GrpcExceptionHelper.CreateRpcException(result.Message, result.ErrorCode);

            return _mapper.Map<CreateFollowResponse>(result.Data);
        }

        public override async Task<Empty> DeleteFollow(DeleteFollowRequest request, ServerCallContext context)
        {
            if (!Guid.TryParse(request.Id, out var followId))
                throw GrpcExceptionHelper.CreateRpcException(ErrorCodes.InvalidInput, FollowErrors.Follow.InvalidId);

            var currentUser = GetCurrentUser();
            var result = await _service.DeleteFollowAsync(followId, currentUser);
            if (!result.Success)
                throw GrpcExceptionHelper.CreateRpcException(result.Message, result.ErrorCode);

            return new Empty();
        }

        private UserInfoDto GetCurrentUser()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            return user == null
                ? throw GrpcExceptionHelper.CreateRpcException(ErrorMessages.Authentication.Unauthorized, ErrorCodes.Unauthorized)
                : user.ToUserInfo();
        }
    }
}