using Waggle.Common.Pagination.Models;
using Waggle.Common.Results.Core;
using Waggle.UserService.Dtos;

namespace Waggle.UserService.Services
{
    public interface IUserService : IUserEventHandler
    {
        Task<Result<PagedResult<UserDto>>> GetUsersAsync(PaginationRequest request);
        Task<Result<UserDto>> GetUserByIdAsync(Guid id);
        Task<Result<List<UserDto>>> GetUsersByIdAsync(UserBatchRequest request);
        Task<Result<UserDto>> CreateUserAsync(UserCreateDto request);
        Task<Result> DeleteUserAsync(Guid id);
    }
}
