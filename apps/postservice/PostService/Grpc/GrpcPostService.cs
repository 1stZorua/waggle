using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Waggle.Common.Auth;
using Waggle.Common.Constants;
using Waggle.Common.Grpc;
using Waggle.Contracts.Post.Grpc;
using Waggle.PostService.Constants;
using Waggle.PostService.Dtos;
using Waggle.PostService.Services;

namespace Waggle.PostService.Grpc
{
    public class GrpcPostService : GrpcPost.GrpcPostBase
    {
        private readonly IPostService _service;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public GrpcPostService(IPostService service, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _service = service;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public override async Task<GetAllPostsResponse> GetAllPosts(GetAllPostsRequest request, ServerCallContext context)
        {
            var paginationRequest = new Common.Pagination.Models.PaginationRequest
            {
                Cursor = request.Cursor,
                PageSize = request.PageSize,
                Direction = request.Direction == PaginationDirection.Backward
                    ? Common.Pagination.Models.PaginationDirection.Backward
                    : Common.Pagination.Models.PaginationDirection.Forward
            };

            var result = await _service.GetAllPostsAsync(paginationRequest);

            if (!result.Success)
                throw GrpcExceptionHelper.CreateRpcException(result.Message, result.ErrorCode);

            var response = new GetAllPostsResponse();
            response.Posts.AddRange(_mapper.Map<IEnumerable<Post>>(result.Data!.Items));
            response.PageInfo = _mapper.Map<PageInfo>(result.Data.PageInfo);

            return response;
        }

        public override async Task<GetPostByIdResponse> GetPostById(GetPostByIdRequest request, ServerCallContext context)
        {
            if (!Guid.TryParse(request.Id, out var postId))
                throw GrpcExceptionHelper.CreateRpcException(ErrorCodes.InvalidInput, PostErrors.Post.InvalidId);

            var result = await _service.GetPostByIdAsync(postId);
            if (!result.Success)
                throw GrpcExceptionHelper.CreateRpcException(result.Message, result.ErrorCode);

            return _mapper.Map<GetPostByIdResponse>(result.Data);
        }

        public override async Task<CreatePostResponse> CreatePost(CreatePostRequest request, ServerCallContext context)
        {

            var dto = _mapper.Map<PostCreateDto>(request);

            var currentUser = GetCurrentUser();
            var result = await _service.CreatePostAsync(dto, currentUser);

            if (!result.Success)
                throw GrpcExceptionHelper.CreateRpcException(result.Message, result.ErrorCode);

            return _mapper.Map<CreatePostResponse>(result.Data);
        }

        public override async Task<Empty> DeletePost(DeletePostRequest request, ServerCallContext context)
        {
            if (!Guid.TryParse(request.Id, out var postId))
                throw GrpcExceptionHelper.CreateRpcException(ErrorCodes.InvalidInput, PostErrors.Post.InvalidId);

            var currentUser = GetCurrentUser();
            var result = await _service.DeletePostAsync(postId, currentUser);
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
