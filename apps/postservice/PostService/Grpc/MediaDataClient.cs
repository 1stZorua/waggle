using Grpc.Core;
using Waggle.Common.Results.Core;
using Waggle.Contracts.Media.Interfaces;
using Waggle.Common.Grpc;
using Waggle.Contracts.Media.Grpc;

namespace Waggle.PostService.Grpc
{
    public class MediaDataClient : IMediaDataClient
    {
        private readonly GrpcMedia.GrpcMediaClient _client;

        public MediaDataClient(GrpcMedia.GrpcMediaClient client)
        {
            _client = client;
        }

        public async Task<Result<GetMediaByIdResponse>> GetMediaByIdAsync(GetMediaByIdRequest request)
        {
            try
            {
                var response = await _client.GetMediaByIdAsync(request);
                return Result<GetMediaByIdResponse>.Ok(response);
            }
            catch (RpcException ex)
            {
                return GrpcExceptionHelper.HandleRpcException<GetMediaByIdResponse>(ex);
            }
        }

        public async Task<Result<GetMediaByIdsResponse>> GetMediaByIdsAsync(GetMediaByIdsRequest request)
        {
            try
            {
                var response = await _client.GetMediaByIdsAsync(request);
                return Result<GetMediaByIdsResponse>.Ok(response);
            }
            catch (RpcException ex)
            {
                return GrpcExceptionHelper.HandleRpcException<GetMediaByIdsResponse>(ex);
            }
        }

        public async Task<Result<GetMediaUrlResponse>> GetMediaUrlAsync(GetMediaUrlRequest request)
        {
            try
            {
                var response = await _client.GetMediaUrlAsync(request);
                return Result<GetMediaUrlResponse>.Ok(response);
            }
            catch (RpcException ex)
            {
                return GrpcExceptionHelper.HandleRpcException<GetMediaUrlResponse>(ex);
            }
        }

        public async Task<Result<GetMediaUrlsResponse>> GetMediaUrlsAsync(GetMediaUrlsRequest request)
        {
            try
            {
                var response = await _client.GetMediaUrlsAsync(request);
                return Result<GetMediaUrlsResponse>.Ok(response);
            }
            catch (RpcException ex)
            {
                return GrpcExceptionHelper.HandleRpcException<GetMediaUrlsResponse>(ex);
            }
        }

        public async Task<Result<UploadMediaResponse>> UploadMediaAsync(UploadMediaRequest request)
        {
            try
            {
                var response = await _client.UploadMediaAsync(request);
                return Result<UploadMediaResponse>.Ok(response);
            }
            catch (RpcException ex)
            {
                return GrpcExceptionHelper.HandleRpcException<UploadMediaResponse>(ex);
            }
        }

        public async Task<Result<GetAllMediaResponse>> GetAllMediaAsync(GetAllMediaRequest request)
        {
            try
            {
                var response = await _client.GetAllMediaAsync(request);
                return Result<GetAllMediaResponse>.Ok(response);
            }
            catch (RpcException ex)
            {
                return GrpcExceptionHelper.HandleRpcException<GetAllMediaResponse>(ex);
            }
        }

        public async Task<Result> DeleteMediaAsync(DeleteMediaRequest request)
        {
            try
            {
                await _client.DeleteMediaAsync(request);
                return Result.Ok();
            }
            catch (RpcException ex)
            {
                return GrpcExceptionHelper.HandleRpcException(ex);
            }
        }
    }
}