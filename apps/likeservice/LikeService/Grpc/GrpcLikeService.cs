using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Waggle.Common.Auth;
using Waggle.Common.Constants;
using Waggle.Common.Grpc;
using Waggle.Contracts.Like.Grpc;
using Waggle.LikeService.Constants;
using Waggle.LikeService.Dtos;
using Waggle.LikeService.Services;

namespace Waggle.LikeService.Grpc
{
    public class GrpcLikeService : GrpcLike.GrpcLikeBase
    {
        private readonly ILikeService _service;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public GrpcLikeService(ILikeService service, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _service = service;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public override async Task<GetLikesResponse> GetLikes(GetLikesRequest request, ServerCallContext context)
        {
            var paginationRequest = new Common.Pagination.Models.PaginationRequest
            {
                Cursor = request.Cursor,
                PageSize = request.PageSize,
                Direction = request.Direction == PaginationDirection.Backward
                    ? Common.Pagination.Models.PaginationDirection.Backward
                    : Common.Pagination.Models.PaginationDirection.Forward
            };

            var result = await _service.GetLikesAsync(paginationRequest);

            if (!result.Success)
                throw GrpcExceptionHelper.CreateRpcException(result.Message, result.ErrorCode);

            var response = new GetLikesResponse();
            response.Likes.AddRange(_mapper.Map<IEnumerable<Like>>(result.Data!.Items));
            response.PageInfo = _mapper.Map<PageInfo>(result.Data.PageInfo);

            return response;
        }

        public override async Task<GetLikeByIdResponse> GetLikeById(GetLikeByIdRequest request, ServerCallContext context)
        {
            if (!Guid.TryParse(request.Id, out var likeId))
                throw GrpcExceptionHelper.CreateRpcException(ErrorCodes.InvalidInput, LikeErrors.Like.InvalidId);

            var result = await _service.GetLikeByIdAsync(likeId);
            if (!result.Success)
                throw GrpcExceptionHelper.CreateRpcException(result.Message, result.ErrorCode);

            return _mapper.Map<GetLikeByIdResponse>(result.Data);
        }

        public override async Task<GetLikesByUserIdResponse> GetLikesByUserId(GetLikesByUserIdRequest request, ServerCallContext context)
        {
            if (!Guid.TryParse(request.UserId, out var userId))
                throw GrpcExceptionHelper.CreateRpcException(ErrorCodes.InvalidInput, LikeErrors.Like.InvalidId);

            var paginationRequest = new Common.Pagination.Models.PaginationRequest
            {
                Cursor = request.Cursor,
                PageSize = request.PageSize,
                Direction = request.Direction == PaginationDirection.Backward
                    ? Common.Pagination.Models.PaginationDirection.Backward
                    : Common.Pagination.Models.PaginationDirection.Forward
            };

            var result = await _service.GetLikesByUserIdAsync(userId, paginationRequest);

            if (!result.Success)
                throw GrpcExceptionHelper.CreateRpcException(result.Message, result.ErrorCode);

            var response = new GetLikesByUserIdResponse();
            response.Likes.AddRange(_mapper.Map<IEnumerable<Like>>(result.Data!.Items));
            response.PageInfo = _mapper.Map<PageInfo>(result.Data.PageInfo);

            return response;
        }

        public override async Task<GetLikesByTargetIdResponse> GetLikesByTargetId(GetLikesByTargetIdRequest request, ServerCallContext context)
        {
            if (!Guid.TryParse(request.TargetId, out var targetId))
                throw GrpcExceptionHelper.CreateRpcException(ErrorCodes.InvalidInput, LikeErrors.Like.InvalidId);

            var paginationRequest = new Common.Pagination.Models.PaginationRequest
            {
                Cursor = request.Cursor,
                PageSize = request.PageSize,
                Direction = request.Direction == PaginationDirection.Backward
                    ? Common.Pagination.Models.PaginationDirection.Backward
                    : Common.Pagination.Models.PaginationDirection.Forward
            };

            var result = await _service.GetLikesByTargetAsync(targetId, paginationRequest);

            if (!result.Success)
                throw GrpcExceptionHelper.CreateRpcException(result.Message, result.ErrorCode);

            var response = new GetLikesByTargetIdResponse();
            response.Likes.AddRange(_mapper.Map<IEnumerable<Like>>(result.Data!.Items));
            response.PageInfo = _mapper.Map<PageInfo>(result.Data.PageInfo);

            return response;
        }

        public override async Task<GetLikeCountResponse> GetLikeCount(GetLikeCountRequest request, ServerCallContext context)
        {
            if (!Guid.TryParse(request.TargetId, out var targetId))
                throw GrpcExceptionHelper.CreateRpcException(ErrorCodes.InvalidInput, LikeErrors.Like.InvalidId);

            var result = await _service.GetLikeCountAsync(targetId);

            if (!result.Success)
                throw GrpcExceptionHelper.CreateRpcException(result.Message, result.ErrorCode);

            return new GetLikeCountResponse
            {
                Count = result.Data
            };
        }

        public override async Task<HasLikedResponse> HasLiked(HasLikedRequest request, ServerCallContext context)
        {
            if (!Guid.TryParse(request.UserId, out var userId))
                throw GrpcExceptionHelper.CreateRpcException(ErrorCodes.InvalidInput, LikeErrors.Like.InvalidId);

            if (!Guid.TryParse(request.TargetId, out var targetId))
                throw GrpcExceptionHelper.CreateRpcException(ErrorCodes.InvalidInput, LikeErrors.Like.InvalidId);

            var result = await _service.HasLikedAsync(userId, targetId);

            if (!result.Success)
                throw GrpcExceptionHelper.CreateRpcException(result.Message, result.ErrorCode);

            return _mapper.Map<HasLikedResponse>(result.Data);
        }

        public override async Task<CreateLikeResponse> CreateLike(CreateLikeRequest request, ServerCallContext context)
        {
            var dto = _mapper.Map<LikeCreateDto>(request);
            var currentUser = GetCurrentUser();
            var result = await _service.CreateLikeAsync(dto, currentUser);

            if (!result.Success)
                throw GrpcExceptionHelper.CreateRpcException(result.Message, result.ErrorCode);

            return _mapper.Map<CreateLikeResponse>(result.Data);
        }

        public override async Task<Empty> DeleteLike(DeleteLikeRequest request, ServerCallContext context)
        {
            if (!Guid.TryParse(request.Id, out var likeId))
                throw GrpcExceptionHelper.CreateRpcException(ErrorCodes.InvalidInput, LikeErrors.Like.InvalidId);

            var currentUser = GetCurrentUser();
            var result = await _service.DeleteLikeAsync(likeId, currentUser);
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