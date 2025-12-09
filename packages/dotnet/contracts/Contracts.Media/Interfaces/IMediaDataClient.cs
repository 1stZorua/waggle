using Waggle.Common.Results.Core;
using Waggle.Contracts.Media.Grpc;

namespace Waggle.Contracts.Media.Interfaces
{
    public interface IMediaDataClient
    {
        Task<Result<GetAllMediaResponse>> GetAllMediaAsync(GetAllMediaRequest request);
        Task<Result<GetMediaByIdResponse>> GetMediaByIdAsync(GetMediaByIdRequest request);
        Task<Result<GetMediaByIdsResponse>> GetMediaByIdsAsync(GetMediaByIdsRequest request);
        Task<Result<GetMediaUrlResponse>> GetMediaUrlAsync(GetMediaUrlRequest request);
        Task<Result<GetMediaUrlsResponse>> GetMediaUrlsAsync(GetMediaUrlsRequest request);
        Task<Result<UploadMediaResponse>> UploadMediaAsync(UploadMediaRequest request);
        Task<Result> DeleteMediaAsync(DeleteMediaRequest request);
    }
}
