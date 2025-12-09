using Waggle.Common.Results.Core;
using Waggle.Contracts.Media.Grpc;
using Waggle.Contracts.Media.Interfaces;

namespace Waggle.Contracts.Media.Extensions
{
    public static class MediaDataClientExtensions
    {
        public static async Task<Result<GetMediaByIdResponse>> GetMediaByIdAsync(
            this IMediaDataClient client,
            Guid id)
        {
            return await client.GetMediaByIdAsync(new GetMediaByIdRequest { Id = id.ToString() });
        }

        public static async Task<Result<GetMediaUrlResponse>> GetMediaUrlAsync(
            this IMediaDataClient client,
            Guid id)
        {
            return await client.GetMediaUrlAsync(new GetMediaUrlRequest { Id = id.ToString() });
        }

        public static async Task<Result> DeleteMediaAsync(
            this IMediaDataClient client,
            Guid id)
        {
            return await client.DeleteMediaAsync(new DeleteMediaRequest { Id = id.ToString() });
        }
    }
}
