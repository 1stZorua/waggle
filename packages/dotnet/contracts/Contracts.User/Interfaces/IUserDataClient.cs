using Waggle.Common.Results.Core;
using Waggle.Contracts.User.Grpc;

namespace Waggle.Contracts.User.Interfaces
{
    public interface IUserDataClient
    {
        Task<Result<GetAllUsersResponse>> GetAllUsersAsync(GetAllUsersRequest request);
        Task<Result<GetUserByIdResponse>> GetUserByIdAsync(GetUserByIdRequest request);
        Task<Result<CreateUserResponse>> CreateUserAsync(CreateUserRequest request);
        Task<Result> DeleteUserAsync(DeleteUserRequest request);
    }
}
