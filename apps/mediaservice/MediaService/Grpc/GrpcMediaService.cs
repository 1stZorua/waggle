using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Waggle.Common.Auth;
using Waggle.Common.Constants;
using Waggle.Common.Grpc;
using Waggle.Contracts.Media.Grpc;
using Waggle.MediaService.Services;
using Waggle.MediaService.Constants;
using Waggle.MediaService.Dtos;

namespace Waggle.MediaService.Grpc
{
    public class GrpcMediaService : GrpcMedia.GrpcMediaBase
    {
        private readonly IMediaService _service;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public GrpcMediaService(IMediaService service, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _service = service;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public override async Task<GetAllMediaResponse> GetAllMedia(GetAllMediaRequest request, ServerCallContext context)
        {
            var paginationRequest = new Common.Pagination.Models.PaginationRequest
            {
                Cursor = request.Cursor,
                PageSize = request.PageSize,
                Direction = request.Direction == PaginationDirection.Backward
                    ? Common.Pagination.Models.PaginationDirection.Backward
                    : Common.Pagination.Models.PaginationDirection.Forward
            };

            var result = await _service.GetAllMediaAsync(paginationRequest);

            if (!result.Success)
                throw GrpcExceptionHelper.CreateRpcException(result.Message, result.ErrorCode);

            var response = new GetAllMediaResponse();
            response.Media.AddRange(_mapper.Map<IEnumerable<Media>>(result.Data!.Items));
            response.PageInfo = _mapper.Map<PageInfo>(result.Data.PageInfo);

            return response;
        }

        public override async Task<GetMediaByIdResponse> GetMediaById(GetMediaByIdRequest request, ServerCallContext context)
        {
            if (!Guid.TryParse(request.Id, out var mediaId))
                throw GrpcExceptionHelper.CreateRpcException(ErrorCodes.InvalidInput, MediaErrors.Media.InvalidId);

            var result = await _service.GetMediaByIdAsync(mediaId);
            if (!result.Success)
                throw GrpcExceptionHelper.CreateRpcException(result.Message, result.ErrorCode);

            return _mapper.Map<GetMediaByIdResponse>(result.Data);
        }

        public override async Task<GetMediaByIdsResponse> GetMediaByIds(GetMediaByIdsRequest request, ServerCallContext context)
        {
            var batchRequest = _mapper.Map<MediaBatchRequest>(request);
            var result = await _service.GetMediaByIdsAsync(batchRequest);

            if (!result.Success)
                throw GrpcExceptionHelper.CreateRpcException(result.Message, result.ErrorCode);

            return _mapper.Map<GetMediaByIdsResponse>(result.Data);
        }

        public override async Task<GetMediaUrlResponse> GetMediaUrl(GetMediaUrlRequest request, ServerCallContext context)
        {
            if (!Guid.TryParse(request.Id, out var mediaId))
                throw GrpcExceptionHelper.CreateRpcException(ErrorCodes.InvalidInput, MediaErrors.Media.InvalidId);

            var result = await _service.GetPresignedMediaUrlAsync(mediaId);
            if (!result.Success)
                throw GrpcExceptionHelper.CreateRpcException(result.Message, result.ErrorCode);

            return _mapper.Map<GetMediaUrlResponse>(result.Data);
        }

        public override async Task<GetMediaUrlsResponse> GetMediaUrls(GetMediaUrlsRequest request, ServerCallContext context)
        {
            var batchRequest = _mapper.Map<MediaBatchRequest>(request);
            var result = await _service.GetPresignedMediaUrlsAsync(batchRequest);

            if (!result.Success)
                throw GrpcExceptionHelper.CreateRpcException(result.Message, result.ErrorCode);

            var response = new GetMediaUrlsResponse();

            foreach (var kvp in result.Data!)
            {
                response.Urls.Add(kvp.Key.ToString(), new MediaUrl
                {
                    Url = kvp.Value.Url,
                    ExpiresAt = Timestamp.FromDateTime(DateTime.SpecifyKind(kvp.Value.ExpiresAt, DateTimeKind.Utc))
                });
            }

            return response;
        }

        public override async Task<UploadMediaResponse> UploadMedia(UploadMediaRequest request, ServerCallContext context)
        {

            var dto = _mapper.Map<MediaCreateDto>(request);

            var currentUser = GetCurrentUser();
            var result = await _service.UploadMediaAsync(dto, currentUser);

            if (!result.Success)
                throw GrpcExceptionHelper.CreateRpcException(result.Message, result.ErrorCode);

            return _mapper.Map<UploadMediaResponse>(result.Data);
        }

        public override async Task<Empty> DeleteMedia(DeleteMediaRequest request, ServerCallContext context)
        {
            if (!Guid.TryParse(request.Id, out var postId))
                throw GrpcExceptionHelper.CreateRpcException(ErrorCodes.InvalidInput, MediaErrors.Media.InvalidId);

            var currentUser = GetCurrentUser();
            var result = await _service.DeleteMediaAsync(postId, currentUser);
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
