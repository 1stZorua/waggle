using UserService.Models;

namespace UserService.Data
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllUsers();
        Task<User?> GetUserById(Guid id);
        Task AddUser(User user);
    }   
}
