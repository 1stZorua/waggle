using AutoMapper;
using UserService.Data;
using UserService.Dtos;
using UserService.Models;
using Waggle.Common.Constants;
using Waggle.Common.Models;

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

        public async Task<Result<IEnumerable<UserReadDto>>> GetAllUsers()
        {
            var users = await _repo.GetAllUsers();
            var dtos = _mapper.Map<IEnumerable<UserReadDto>>(users);
            return Result<IEnumerable<UserReadDto>>.Ok(dtos);
        }

        public async Task<Result<UserReadDto>> GetUserById(int id)
        {
            var user = await _repo.GetUserById(id);
            if (user == null)
                return Result<UserReadDto>.Fail("User not found", ErrorCodes.NotFound);

            return Result<UserReadDto>.Ok(_mapper.Map<UserReadDto>(user));
        }

        public async Task<Result<UserReadDto>> CreateUser(UserCreateDto dto)
        {
            var user = _mapper.Map<User>(dto);
            await _repo.AddUser(user);
            return Result<UserReadDto>.Ok(_mapper.Map<UserReadDto>(user));
        }
    }
}
