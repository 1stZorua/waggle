using Waggle.Common.Results.Core;
using Waggle.Contracts.Favorite.Interfaces;
using Waggle.Contracts.Favorite.Grpc;

namespace Waggle.Contracts.Favorite.Extensions
{
    public static class FavoriteDataClientExtensions
    {
        public static async Task<Result<GetFavoriteByIdResponse>> GetFavoriteByIdAsync(
            this IFavoriteDataClient client,
            Guid id)
        {
            return await client.GetFavoriteByIdAsync(new GetFavoriteByIdRequest { Id = id.ToString() });
        }

        public static async Task<Result> DeleteFavoriteAsync(
            this IFavoriteDataClient client,
            Guid id)
        {
            return await client.DeleteFavoriteAsync(new DeleteFavoriteRequest { Id = id.ToString() });
        }
    }
}
