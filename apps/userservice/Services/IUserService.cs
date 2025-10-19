using UserService.Dtos;
using Waggle.Common.Results;

namespace UserService.Services
{
    public interface IUserService
    {
        Task<Result<IEnumerable<UserDto>>> GetAllUsersAsync();
        Task<Result<UserDto>> GetUserByIdAsync(Guid id);
        Task<Result<UserDto>> CreateUserAsync(UserCreateDto dto);
    }
}
