using Waggle.Common.Pagination.Models;
using Waggle.UserService.Models;

namespace Waggle.UserService.Data
{
    public interface IUserRepository
    {
        Task<PagedResult<User>> GetAllUsersAsync(PaginationRequest request);
        Task<User?> GetUserByIdAsync(Guid id);
        Task AddUserAsync(User user);
        Task DeleteUserAsync(User user);
    }   
}
