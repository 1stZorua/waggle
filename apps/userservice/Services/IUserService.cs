using UserService.Dtos;
using Waggle.Common.Models;

namespace UserService.Services
{
    public interface IUserService
    {
        Task<Result<IEnumerable<UserReadDto>>> GetAllUsers();
        Task<Result<UserReadDto>> GetUserById(int id);
        Task<Result<UserReadDto>> CreateUser(UserCreateDto dto);
    }
}
