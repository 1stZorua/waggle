using Waggle.Common.Results;
using Waggle.Contracts.User.Grpc;
using Waggle.Contracts.User.Interfaces;

namespace Waggle.Contracts.User.Extensions
{
    public static class UserDataClientExtensions
    {
        public static async Task<Result<GetUserByIdResponse>> GetUserByIdAsync(
            this IUserDataClient client,
            Guid id)
        {
            return await client.GetUserByIdAsync(new GetUserByIdRequest { Id = id.ToString() });
        }
    }
}
