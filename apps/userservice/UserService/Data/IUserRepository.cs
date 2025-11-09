using Waggle.Common.Pagination.Models;
using Waggle.UserService.Models;

namespace Waggle.UserService.Data
{
    public interface IUserRepository
    {
        Task<PagedResult<User>> GetAllUsers(PaginationRequest request);
        Task<User?> GetUserById(Guid id);
        Task AddUser(User user);
    }   
}
