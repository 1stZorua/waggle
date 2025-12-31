using Waggle.Common.Pagination.Models;
using Waggle.UserService.Dtos;
using Waggle.UserService.Models;

namespace Waggle.UserService.Data
{
    public interface IUserRepository
    {
        Task<PagedResult<User>> GetUsersAsync(PaginationRequest request);
        Task<User?> GetUserByIdAsync(Guid id);
        Task<List<User>> GetUsersByIdAsync(UserBatchRequest request);
        Task AddUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(User user);
    }   
}
