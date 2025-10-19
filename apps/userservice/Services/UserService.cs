using AutoMapper;
using UserService.Data;
using UserService.Dtos;
using UserService.Models;
using Waggle.Common.Constants;
using Waggle.Common.Results;

namespace UserService.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;
        private readonly IMapper _mapper;

        public UserService(IUserRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<Result<IEnumerable<UserDto>>> GetAllUsersAsync()
        {
            var users = await _repo.GetAllUsers();
            var dtos = _mapper.Map<IEnumerable<UserDto>>(users);
            return Result<IEnumerable<UserDto>>.Ok(dtos);
        }

        public async Task<Result<UserDto>> GetUserByIdAsync(Guid id)
        {
            var user = await _repo.GetUserById(id);
            if (user == null)
                return Result<UserDto>.Fail("User not found", ErrorCodes.NotFound);

            return Result<UserDto>.Ok(_mapper.Map<UserDto>(user));
        }

        public async Task<Result<UserDto>> CreateUserAsync(UserCreateDto dto)
        {
            var user = _mapper.Map<User>(dto);

            user.CreatedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;

            await _repo.AddUser(user);
            return Result<UserDto>.Ok(_mapper.Map<UserDto>(user));
        }
    }
}
