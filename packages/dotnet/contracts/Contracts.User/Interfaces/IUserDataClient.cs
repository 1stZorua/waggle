using Waggle.Common.Results.Core;
using Waggle.Contracts.User.Grpc;

namespace Waggle.Contracts.User.Interfaces
{
    public interface IUserDataClient
    {
        Task<Result<GetUsersResponse>> GetUsersAsync(GetUsersRequest request);
        Task<Result<GetUserByIdResponse>> GetUserByIdAsync(GetUserByIdRequest request);
        Task<Result<GetUsersByIdResponse>> GetUsersByIdAsync(GetUsersByIdRequest request);
        Task<Result<CreateUserResponse>> CreateUserAsync(CreateUserRequest request);
        Task<Result> DeleteUserAsync(DeleteUserRequest request);
    }
}
