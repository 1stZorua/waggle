using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Waggle.Common.Auth;
using Waggle.Common.Constants;
using Waggle.Common.Grpc;
using Waggle.Contracts.Favorite.Grpc;
using Waggle.FavoriteService.Constants;
using Waggle.FavoriteService.Dtos;
using Waggle.FavoriteService.Services;

namespace Waggle.FavoriteService.Grpc
{
    public class GrpcFavoriteService : GrpcFavorite.GrpcFavoriteBase
    {
        private readonly IFavoriteService _service;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public GrpcFavoriteService(IFavoriteService service, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _service = service;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public override async Task<GetFavoritesResponse> GetFavorites(GetFavoritesRequest request, ServerCallContext context)
        {
            var paginationRequest = new Common.Pagination.Models.PaginationRequest
            {
                Cursor = request.Cursor,
                PageSize = request.PageSize,
                Direction = request.Direction == PaginationDirection.Backward
                    ? Common.Pagination.Models.PaginationDirection.Backward
                    : Common.Pagination.Models.PaginationDirection.Forward
            };

            var result = await _service.GetFavoritesAsync(paginationRequest);

            if (!result.Success)
                throw GrpcExceptionHelper.CreateRpcException(result.Message, result.ErrorCode);

            var response = new GetFavoritesResponse();
            response.Favorites.AddRange(_mapper.Map<IEnumerable<Favorite>>(result.Data!.Items));
            response.PageInfo = _mapper.Map<PageInfo>(result.Data.PageInfo);

            return response;
        }

        public override async Task<GetFavoriteByIdResponse> GetFavoriteById(GetFavoriteByIdRequest request, ServerCallContext context)
        {
            if (!Guid.TryParse(request.Id, out var favoriteId))
                throw GrpcExceptionHelper.CreateRpcException(ErrorCodes.InvalidInput, FavoriteErrors.Favorite.InvalidId);

            var result = await _service.GetFavoriteByIdAsync(favoriteId);
            if (!result.Success)
                throw GrpcExceptionHelper.CreateRpcException(result.Message, result.ErrorCode);

            return _mapper.Map<GetFavoriteByIdResponse>(result.Data);
        }

        public override async Task<GetFavoritesByUserIdResponse> GetFavoritesByUserId(GetFavoritesByUserIdRequest request, ServerCallContext context)
        {
            if (!Guid.TryParse(request.UserId, out var userId))
                throw GrpcExceptionHelper.CreateRpcException(ErrorCodes.InvalidInput, FavoriteErrors.Favorite.InvalidId);

            var paginationRequest = new Common.Pagination.Models.PaginationRequest
            {
                Cursor = request.Cursor,
                PageSize = request.PageSize,
                Direction = request.Direction == PaginationDirection.Backward
                    ? Common.Pagination.Models.PaginationDirection.Backward
                    : Common.Pagination.Models.PaginationDirection.Forward
            };

            var result = await _service.GetFavoritesByUserIdAsync(userId, paginationRequest);

            if (!result.Success)
                throw GrpcExceptionHelper.CreateRpcException(result.Message, result.ErrorCode);

            var response = new GetFavoritesByUserIdResponse();
            response.Favorites.AddRange(_mapper.Map<IEnumerable<Favorite>>(result.Data!.Items));
            response.PageInfo = _mapper.Map<PageInfo>(result.Data.PageInfo);

            return response;
        }

        public override async Task<GetFavoritesByTargetResponse> GetFavoritesByTarget(GetFavoritesByTargetRequest request, ServerCallContext context)
        {
            if (!Guid.TryParse(request.TargetId, out var targetId))
                throw GrpcExceptionHelper.CreateRpcException(ErrorCodes.InvalidInput, FavoriteErrors.Favorite.InvalidId);

            var paginationRequest = new Common.Pagination.Models.PaginationRequest
            {
                Cursor = request.Cursor,
                PageSize = request.PageSize,
                Direction = request.Direction == PaginationDirection.Backward
                    ? Common.Pagination.Models.PaginationDirection.Backward
                    : Common.Pagination.Models.PaginationDirection.Forward
            };

            var result = await _service.GetFavoritesByTargetAsync(targetId, paginationRequest);

            if (!result.Success)
                throw GrpcExceptionHelper.CreateRpcException(result.Message, result.ErrorCode);

            var response = new GetFavoritesByTargetResponse();
            response.Favorites.AddRange(_mapper.Map<IEnumerable<Favorite>>(result.Data!.Items));
            response.PageInfo = _mapper.Map<PageInfo>(result.Data.PageInfo);

            return response;
        }

        public override async Task<GetFavoriteCountsResponse> GetFavoriteCounts(GetFavoriteCountsRequest request, ServerCallContext context)
        {
            var targetIds = request.TargetIds
                .Select(id => Guid.TryParse(id, out var guid) ? guid : (Guid?)null)
                .Where(id => id.HasValue)
                .Select(id => id!.Value)
                .ToList();

            if (targetIds.Count == 0)
                return new GetFavoriteCountsResponse();

            var result = await _service.GetFavoriteCountsAsync(targetIds);

            if (!result.Success)
                throw GrpcExceptionHelper.CreateRpcException(result.Message, result.ErrorCode);

            var response = new GetFavoriteCountsResponse();
            foreach (var kvp in result.Data!)
            {
                response.Counts.Add(new FavoriteCountEntry
                {
                    TargetId = kvp.Key.ToString(),
                    Count = kvp.Value
                });
            }

            return response;
        }

        public override async Task<HasFavoritedResponse> HasFavorited(HasFavoritedRequest request, ServerCallContext context)
        {
            if (!Guid.TryParse(request.UserId, out var userId))
                throw GrpcExceptionHelper.CreateRpcException(ErrorCodes.InvalidInput, FavoriteErrors.Favorite.InvalidId);

            if (!Guid.TryParse(request.TargetId, out var targetId))
                throw GrpcExceptionHelper.CreateRpcException(ErrorCodes.InvalidInput, FavoriteErrors.Favorite.InvalidId);

            var result = await _service.HasFavoritedAsync(userId, targetId);

            if (!result.Success)
                throw GrpcExceptionHelper.CreateRpcException(result.Message, result.ErrorCode);

            return _mapper.Map<HasFavoritedResponse>(result.Data);
        }

        public override async Task<CreateFavoriteResponse> CreateFavorite(CreateFavoriteRequest request, ServerCallContext context)
        {
            var dto = _mapper.Map<FavoriteCreateDto>(request);
            var currentUser = GetCurrentUser();
            var result = await _service.CreateFavoriteAsync(dto, currentUser);

            if (!result.Success)
                throw GrpcExceptionHelper.CreateRpcException(result.Message, result.ErrorCode);

            return _mapper.Map<CreateFavoriteResponse>(result.Data);
        }

        public override async Task<Empty> DeleteFavorite(DeleteFavoriteRequest request, ServerCallContext context)
        {
            if (!Guid.TryParse(request.Id, out var favoriteId))
                throw GrpcExceptionHelper.CreateRpcException(ErrorCodes.InvalidInput, FavoriteErrors.Favorite.InvalidId);

            var currentUser = GetCurrentUser();
            var result = await _service.DeleteFavoriteAsync(favoriteId, currentUser);
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