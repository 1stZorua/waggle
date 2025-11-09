using Waggle.Common.Pagination.Models;
using Waggle.Common.Results.Core;
using Waggle.UserService.Dtos;

namespace Waggle.UserService.Services
{
    public interface IUserService
    {
        Task<Result<PagedResult<UserDto>>> GetAllUsersAsync(PaginationRequest request);
        Task<Result<UserDto>> GetUserByIdAsync(Guid id);
        Task<Result<UserDto>> CreateUserAsync(UserCreateDto dto);
        Task<Result<UserDto>> CreateUserFromEventAsync(UserCreateDto dto);
    }
}
