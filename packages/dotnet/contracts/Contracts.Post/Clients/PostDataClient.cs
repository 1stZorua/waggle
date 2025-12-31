using Grpc.Core;
using Waggle.Common.Grpc;
using Waggle.Common.Results.Core;
using Waggle.Contracts.Post.Grpc;
using Waggle.Contracts.Post.Interfaces;

namespace Waggle.Contracts.Post.Clients
{
    public class PostDataClient : IPostDataClient
    {
        private readonly GrpcPost.GrpcPostClient _client;

        public PostDataClient(GrpcPost.GrpcPostClient client)
        {
            _client = client;
        }

        public async Task<Result<GetPostsResponse>> GetPostsAsync(GetPostsRequest request)
        {
            try
            {
                var response = await _client.GetPostsAsync(request);
                return Result<GetPostsResponse>.Ok(response);
            }
            catch (RpcException ex)
            {
                return GrpcExceptionHelper.HandleRpcException<GetPostsResponse>(ex);
            }
        }

        public async Task<Result<GetPostByIdResponse>> GetPostByIdAsync(GetPostByIdRequest request)
        {
            try
            {
                var response = await _client.GetPostByIdAsync(request);
                return Result<GetPostByIdResponse>.Ok(response);
            }
            catch (RpcException ex)
            {
                return GrpcExceptionHelper.HandleRpcException<GetPostByIdResponse>(ex);
            }
        }

        public async Task<Result<GetPostsByUserIdResponse>> GetPostsByUserIdAsync(GetPostsByUserIdRequest request)
        {
            try
            {
                var response = await _client.GetPostsByUserIdAsync(request);
                return Result<GetPostsByUserIdResponse>.Ok(response);
            }
            catch (RpcException ex)
            {
                return GrpcExceptionHelper.HandleRpcException<GetPostsByUserIdResponse>(ex);
            }
        }

        public async Task<Result<GetPostCountsResponse>> GetPostCountsAsync(GetPostCountsRequest request)
        {
            try
            {
                var response = await _client.GetPostCountsAsync(request);
                return Result<GetPostCountsResponse>.Ok(response);
            }
            catch (RpcException ex)
            {
                return GrpcExceptionHelper.HandleRpcException<GetPostCountsResponse>(ex);
            }
        }

        public async Task<Result<CreatePostResponse>> CreatePostAsync(CreatePostRequest request)
        {
            try
            {
                var response = await _client.CreatePostAsync(request);
                return Result<CreatePostResponse>.Ok(response);
            }
            catch (RpcException ex)
            {
                return GrpcExceptionHelper.HandleRpcException<CreatePostResponse>(ex);
            }
        }

        public async Task<Result<UpdatePostResponse>> UpdatePostAsync(UpdatePostRequest request)
        {
            try
            {
                var response = await _client.UpdatePostAsync(request);
                return Result<UpdatePostResponse>.Ok(response);
            }
            catch (RpcException ex)
            {
                return GrpcExceptionHelper.HandleRpcException<UpdatePostResponse>(ex);
            }
        }

        public async Task<Result> DeletePostAsync(DeletePostRequest request)
        {
            try
            {
                await _client.DeletePostAsync(request);
                return Result.Ok();
            }
            catch (RpcException ex)
            {
                return GrpcExceptionHelper.HandleRpcException(ex);
            }
        }
    }
}