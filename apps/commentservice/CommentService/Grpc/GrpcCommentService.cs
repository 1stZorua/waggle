using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Waggle.Common.Auth;
using Waggle.Common.Constants;
using Waggle.Common.Grpc;
using Waggle.Contracts.Comment.Grpc;
using Waggle.CommentService.Constants;
using Waggle.CommentService.Dtos;
using Waggle.CommentService.Services;

namespace Waggle.CommentService.Grpc
{
    public class GrpcCommentService : GrpcComment.GrpcCommentBase
    {
        private readonly ICommentService _service;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public GrpcCommentService(ICommentService service, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _service = service;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public override async Task<GetCommentsResponse> GetComments(GetCommentsRequest request, ServerCallContext context)
        {
            var paginationRequest = new Common.Pagination.Models.PaginationRequest
            {
                Cursor = request.Cursor,
                PageSize = request.PageSize,
                Direction = request.Direction == PaginationDirection.Backward
                    ? Common.Pagination.Models.PaginationDirection.Backward
                    : Common.Pagination.Models.PaginationDirection.Forward
            };

            var result = await _service.GetCommentsAsync(paginationRequest);

            if (!result.Success)
                throw GrpcExceptionHelper.CreateRpcException(result.Message, result.ErrorCode);

            var response = new GetCommentsResponse();
            response.Comments.AddRange(_mapper.Map<IEnumerable<Comment>>(result.Data!.Items));
            response.PageInfo = _mapper.Map<PageInfo>(result.Data.PageInfo);

            return response;
        }

        public override async Task<GetCommentByIdResponse> GetCommentById(GetCommentByIdRequest request, ServerCallContext context)
        {
            if (!Guid.TryParse(request.Id, out var commentId))
                throw GrpcExceptionHelper.CreateRpcException(ErrorCodes.InvalidInput, CommentErrors.Comment.InvalidId);

            var result = await _service.GetCommentByIdAsync(commentId);
            if (!result.Success)
                throw GrpcExceptionHelper.CreateRpcException(result.Message, result.ErrorCode);

            return _mapper.Map<GetCommentByIdResponse>(result.Data);
        }

        public override async Task<GetCommentsByPostResponse> GetCommentsByPost(GetCommentsByPostRequest request, ServerCallContext context)
        {
            if (!Guid.TryParse(request.PostId, out var postId))
                throw GrpcExceptionHelper.CreateRpcException(ErrorCodes.InvalidInput, CommentErrors.Comment.InvalidId);

            var paginationRequest = new Common.Pagination.Models.PaginationRequest
            {
                Cursor = request.Cursor,
                PageSize = request.PageSize,
                Direction = request.Direction == PaginationDirection.Backward
                    ? Common.Pagination.Models.PaginationDirection.Backward
                    : Common.Pagination.Models.PaginationDirection.Forward
            };

            var result = await _service.GetCommentsByPostAsync(postId, paginationRequest);

            if (!result.Success)
                throw GrpcExceptionHelper.CreateRpcException(result.Message, result.ErrorCode);

            var response = new GetCommentsByPostResponse();
            response.Comments.AddRange(_mapper.Map<IEnumerable<Comment>>(result.Data!.Items));
            response.PageInfo = _mapper.Map<PageInfo>(result.Data.PageInfo);

            return response;
        }

        public override async Task<GetRepliesResponse> GetReplies(GetRepliesRequest request, ServerCallContext context)
        {
            if (!Guid.TryParse(request.CommentId, out var commentId))
                throw GrpcExceptionHelper.CreateRpcException(ErrorCodes.InvalidInput, CommentErrors.Comment.InvalidId);

            var paginationRequest = new Common.Pagination.Models.PaginationRequest
            {
                Cursor = request.Cursor,
                PageSize = request.PageSize,
                Direction = request.Direction == PaginationDirection.Backward
                    ? Common.Pagination.Models.PaginationDirection.Backward
                    : Common.Pagination.Models.PaginationDirection.Forward
            };

            var result = await _service.GetRepliesAsync(commentId, paginationRequest);

            if (!result.Success)
                throw GrpcExceptionHelper.CreateRpcException(result.Message, result.ErrorCode);

            var response = new GetRepliesResponse();
            response.Comments.AddRange(_mapper.Map<IEnumerable<Comment>>(result.Data!.Items));
            response.PageInfo = _mapper.Map<PageInfo>(result.Data.PageInfo);

            return response;
        }

        public override async Task<GetCommentsByUserResponse> GetCommentsByUser(GetCommentsByUserRequest request, ServerCallContext context)
        {
            if (!Guid.TryParse(request.UserId, out var userId))
                throw GrpcExceptionHelper.CreateRpcException(ErrorCodes.InvalidInput, CommentErrors.Comment.InvalidId);

            var paginationRequest = new Common.Pagination.Models.PaginationRequest
            {
                Cursor = request.Cursor,
                PageSize = request.PageSize,
                Direction = request.Direction == PaginationDirection.Backward
                    ? Common.Pagination.Models.PaginationDirection.Backward
                    : Common.Pagination.Models.PaginationDirection.Forward
            };

            var result = await _service.GetCommentsByUserAsync(userId, paginationRequest);

            if (!result.Success)
                throw GrpcExceptionHelper.CreateRpcException(result.Message, result.ErrorCode);

            var response = new GetCommentsByUserResponse();
            response.Comments.AddRange(_mapper.Map<IEnumerable<Comment>>(result.Data!.Items));
            response.PageInfo = _mapper.Map<PageInfo>(result.Data.PageInfo);

            return response;
        }

        public override async Task<GetCommentCountsResponse> GetCommentCounts(GetCommentCountsRequest request, ServerCallContext context)
        {
            var postIds = request.PostIds
                .Select(id => Guid.TryParse(id, out var guid) ? guid : (Guid?)null)
                .Where(id => id.HasValue)
                .Select(id => id!.Value)
                .ToList();

            if (postIds.Count == 0)
                return new GetCommentCountsResponse();

            var result = await _service.GetCommentCountsAsync(postIds);

            if (!result.Success)
                throw GrpcExceptionHelper.CreateRpcException(result.Message, result.ErrorCode);

            var response = new GetCommentCountsResponse();
            foreach (var kvp in result.Data!)
            {
                response.Counts.Add(new CommentCountEntry
                {
                    PostId = kvp.Key.ToString(),
                    Count = kvp.Value
                });
            }

            return response;
        }

        public override async Task<GetReplyCountsResponse> GetReplyCounts(GetReplyCountsRequest request, ServerCallContext context)
        {
            var commentIds = request.CommentIds
                .Select(id => Guid.TryParse(id, out var guid) ? guid : (Guid?)null)
                .Where(id => id.HasValue)
                .Select(id => id!.Value)
                .ToList();

            if (commentIds.Count == 0)
                return new GetReplyCountsResponse();

            var result = await _service.GetReplyCountsAsync(commentIds);

            if (!result.Success)
                throw GrpcExceptionHelper.CreateRpcException(result.Message, result.ErrorCode);

            var response = new GetReplyCountsResponse();
            foreach (var kvp in result.Data!)
            {
                response.Counts.Add(new ReplyCountEntry
                {
                    CommentId = kvp.Key.ToString(),
                    Count = kvp.Value
                });
            }

            return response;
        }

        public override async Task<CreateCommentResponse> CreateComment(CreateCommentRequest request, ServerCallContext context)
        {
            var dto = _mapper.Map<CommentCreateDto>(request);
            var currentUser = GetCurrentUser();
            var result = await _service.CreateCommentAsync(dto, currentUser);

            if (!result.Success)
                throw GrpcExceptionHelper.CreateRpcException(result.Message, result.ErrorCode);

            return _mapper.Map<CreateCommentResponse>(result.Data);
        }

        public override async Task<UpdateCommentResponse> UpdateComment(UpdateCommentRequest request, ServerCallContext context)
        {
            if (!Guid.TryParse(request.Id, out var commentId))
                throw GrpcExceptionHelper.CreateRpcException(ErrorCodes.InvalidInput, CommentErrors.Comment.InvalidId);

            var dto = _mapper.Map<CommentUpdateDto>(request);
            var currentUser = GetCurrentUser();
            var result = await _service.UpdateCommentAsync(commentId, dto, currentUser);

            if (!result.Success)
                throw GrpcExceptionHelper.CreateRpcException(result.Message, result.ErrorCode);

            return _mapper.Map<UpdateCommentResponse>(result.Data);
        }

        public override async Task<Empty> DeleteComment(DeleteCommentRequest request, ServerCallContext context)
        {
            if (!Guid.TryParse(request.Id, out var commentId))
                throw GrpcExceptionHelper.CreateRpcException(ErrorCodes.InvalidInput, CommentErrors.Comment.InvalidId);

            var currentUser = GetCurrentUser();
            var result = await _service.DeleteCommentAsync(commentId, currentUser);
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