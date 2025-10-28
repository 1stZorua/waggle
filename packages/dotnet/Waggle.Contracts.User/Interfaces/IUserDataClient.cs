using Waggle.Common.Results;
using Waggle.Contracts.User.Grpc;

namespace Waggle.Contracts.User.Interfaces
{
    public interface IUserDataClient
    {
        Task<Result<GetAllUsersResponse>> GetAllUsersAsync();
        Task<Result<GetUserByIdResponse>> GetUserByIdAsync(GetUserByIdRequest request);
        Task<Result<CreateUserResponse>> CreateUserAsync(CreateUserRequest request);
    }
}
