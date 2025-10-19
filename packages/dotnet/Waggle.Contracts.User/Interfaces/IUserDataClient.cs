using UserService.Grpc;
using Waggle.Common.Results;

namespace Waggle.Contracts.User.Interfaces
{
    public interface IUserDataClient
    {
        Task<Result<GetAllUsersResponse>> GetAllUsersAsync();
        Task<Result<GetUserByIdResponse>> GetUserByIdAsync(GetUserByIdRequest request);
        Task<Result<CreateUserResponse>> CreateUserAsync(CreateUserRequest request);
    }
}
