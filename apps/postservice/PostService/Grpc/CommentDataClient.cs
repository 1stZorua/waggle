using Grpc.Core;
using Waggle.Common.Grpc;
using Waggle.Common.Results.Core;
using Waggle.Contracts.Comment.Grpc;
using Waggle.Contracts.Comment.Interfaces;

namespace Waggle.PostService.Grpc
{
    public class CommentDataClient : ICommentDataClient
    {
        private readonly GrpcComment.GrpcCommentClient _client;

        public CommentDataClient(GrpcComment.GrpcCommentClient client)
        {
            _client = client;
        }

        public async Task<Result<GetCommentsResponse>> GetCommentsAsync(GetCommentsRequest request)
        {
            try
            {
                var response = await _client.GetCommentsAsync(request);
                return Result<GetCommentsResponse>.Ok(response);
            }
            catch (RpcException ex)
            {
                return GrpcExceptionHelper.HandleRpcException<GetCommentsResponse>(ex);
            }
        }

        public async Task<Result<GetCommentByIdResponse>> GetCommentByIdAsync(GetCommentByIdRequest request)
        {
            try
            {
                var response = await _client.GetCommentByIdAsync(request);
                return Result<GetCommentByIdResponse>.Ok(response);
            }
            catch (RpcException ex)
            {
                return GrpcExceptionHelper.HandleRpcException<GetCommentByIdResponse>(ex);
            }
        }

        public async Task<Result<GetCommentsByPostResponse>> GetCommentsByPostAsync(GetCommentsByPostRequest request)
        {
            try
            {
                var response = await _client.GetCommentsByPostAsync(request);
                return Result<GetCommentsByPostResponse>.Ok(response);
            }
            catch (RpcException ex)
            {
                return GrpcExceptionHelper.HandleRpcException<GetCommentsByPostResponse>(ex);
            }
        }

        public async Task<Result<GetRepliesResponse>> GetRepliesAsync(GetRepliesRequest request)
        {
            try
            {
                var response = await _client.GetRepliesAsync(request);
                return Result<GetRepliesResponse>.Ok(response);
            }
            catch (RpcException ex)
            {
                return GrpcExceptionHelper.HandleRpcException<GetRepliesResponse>(ex);
            }
        }

        public async Task<Result<GetCommentsByUserResponse>> GetCommentsByUserAsync(GetCommentsByUserRequest request)
        {
            try
            {
                var response = await _client.GetCommentsByUserAsync(request);
                return Result<GetCommentsByUserResponse>.Ok(response);
            }
            catch (RpcException ex)
            {
                return GrpcExceptionHelper.HandleRpcException<GetCommentsByUserResponse>(ex);
            }
        }

        public async Task<Result<GetCommentCountResponse>> GetCommentCountAsync(GetCommentCountRequest request)
        {
            try
            {
                var response = await _client.GetCommentCountAsync(request);
                return Result<GetCommentCountResponse>.Ok(response);
            }
            catch (RpcException ex)
            {
                return GrpcExceptionHelper.HandleRpcException<GetCommentCountResponse>(ex);
            }
        }

        public async Task<Result<GetReplyCountResponse>> GetReplyCountAsync(GetReplyCountRequest request)
        {
            try
            {
                var response = await _client.GetReplyCountAsync(request);
                return Result<GetReplyCountResponse>.Ok(response);
            }
            catch (RpcException ex)
            {
                return GrpcExceptionHelper.HandleRpcException<GetReplyCountResponse>(ex);
            }
        }

        public async Task<Result<CreateCommentResponse>> CreateCommentAsync(CreateCommentRequest request)
        {
            try
            {
                var response = await _client.CreateCommentAsync(request);
                return Result<CreateCommentResponse>.Ok(response);
            }
            catch (RpcException ex)
            {
                return GrpcExceptionHelper.HandleRpcException<CreateCommentResponse>(ex);
            }
        }

        public async Task<Result<UpdateCommentResponse>> UpdateCommentAsync(UpdateCommentRequest request)
        {
            try
            {
                var response = await _client.UpdateCommentAsync(request);
                return Result<UpdateCommentResponse>.Ok(response);
            }
            catch (RpcException ex)
            {
                return GrpcExceptionHelper.HandleRpcException<UpdateCommentResponse>(ex);
            }
        }

        public async Task<Result> DeleteCommentAsync(DeleteCommentRequest request)
        {
            try
            {
                await _client.DeleteCommentAsync(request);
                return Result.Ok();
            }
            catch (RpcException ex)
            {
                return GrpcExceptionHelper.HandleRpcException(ex);
            }
        }
    }
}