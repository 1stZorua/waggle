using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Waggle.Common.Constants;
using Waggle.Common.Pagination.Models;
using Waggle.Common.Results.Core;
using Waggle.UserService.Constants;
using Waggle.UserService.Data;
using Waggle.UserService.Dtos;
using Waggle.UserService.Logging;
using Waggle.UserService.Models;

namespace Waggle.UserService.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository repo, IMapper mapper, ILogger<UserService> logger)
        {
            _repo = repo;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<PagedResult<UserDto>>> GetAllUsersAsync(PaginationRequest request)
        {
            try
            {
                var users = await _repo.GetAllUsers(request);

                var pagedResult = new PagedResult<UserDto>
                {
                    Items = _mapper.Map<List<UserDto>>(users.Items),
                    PageInfo = users.PageInfo
                };

                _logger.LogUsersRetrieved(pagedResult.Items.Count);

                return Result<PagedResult<UserDto>>.Ok(pagedResult);
            } 
            catch (Exception ex)
            {
                _logger.LogUsersRetrievalFailed(ex);
                return Result<PagedResult<UserDto>>.Fail(UserErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }

        public async Task<Result<UserDto>> GetUserByIdAsync(Guid id)
        {
            try
            {
                var user = await _repo.GetUserById(id);
                if (user == null)
                {
                    _logger.LogUserNotFound(id);
                    return Result<UserDto>.Fail(UserErrors.User.NotFound, ErrorCodes.NotFound);
                }

                _logger.LogUserRetrieved(id);

                var dto = _mapper.Map<UserDto>(user);
                return Result<UserDto>.Ok(dto);
            } 
            catch (Exception ex)
            {
                _logger.LogUserRetrievalFailed(ex, id);
                return Result<UserDto>.Fail(UserErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }

        public async Task<Result<UserDto>> CreateUserAsync(UserCreateDto dto)
        {
            try
            {
                var existing = await _repo.GetUserById(dto.Id);
                if (existing != null)
                {
                    _logger.LogUserAlreadyExists(dto.Id);
                    return Result<UserDto>.Fail(UserErrors.User.AlreadyExists, ErrorCodes.AlreadyExists);
                }

                var user = _mapper.Map<User>(dto);
                user.CreatedAt = DateTime.UtcNow;
                user.UpdatedAt = DateTime.UtcNow;

                await _repo.AddUser(user);

                _logger.LogUserCreated(dto.Username, dto.Id);

                var result = _mapper.Map<UserDto>(user);
                return Result<UserDto>.Ok(result);
            }
            catch (DbUpdateException ex) when (IsDuplicateKeyError(ex))
            {
                _logger.LogDuplicateKeyError(ex, dto.Id);
                return Result<UserDto>.Fail(UserErrors.User.AlreadyExists, ErrorCodes.AlreadyExists);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogDatabaseUpdateFailed(ex, dto.Id);
                return Result<UserDto>.Fail(UserErrors.User.CreationFailed, ErrorCodes.ServiceFailed);
            }
            catch (Exception ex)
            {
                _logger.LogUserCreationFailed(ex, dto.Id);
                return Result<UserDto>.Fail(UserErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }

        public async Task<Result<UserDto>> CreateUserFromEventAsync(UserCreateDto dto)
        {
            try
            {
                var existing = await _repo.GetUserById(dto.Id);
                if (existing != null)
                {
                    _logger.LogUserExistsFromEvent(dto.Id);
                    return Result<UserDto>.Ok(_mapper.Map<UserDto>(existing));
                }

                var user = _mapper.Map<User>(dto);
                user.CreatedAt = DateTime.UtcNow;
                user.UpdatedAt = DateTime.UtcNow;

                await _repo.AddUser(user);

                _logger.LogUserCreatedFromEvent(dto.Username, dto.Id);

                var result = _mapper.Map<UserDto>(user);
                return Result<UserDto>.Ok(result);
            } 
            catch (DbUpdateException ex) when (IsDuplicateKeyError(ex))
            {
                _logger.LogDuplicateKeyErrorFromEvent(ex, dto.Id);

                var existing = await _repo.GetUserById(dto.Id);
                if (existing != null)
                    return Result<UserDto>.Ok(_mapper.Map<UserDto>(existing));

                return Result<UserDto>.Fail(
                    UserErrors.User.CreationFailed,
                    ErrorCodes.ServiceFailed);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogDatabaseUpdateFailedFromEvent(ex, dto.Id);
                return Result<UserDto>.Fail(UserErrors.User.CreationFailed, ErrorCodes.ServiceFailed);
            }
            catch (Exception ex)
            {
                _logger.LogUserCreationFromEventFailed(ex, dto.Id);
                return Result<UserDto>.Fail(UserErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }

        private bool IsDuplicateKeyError(DbUpdateException ex)
        {
            return ex.InnerException?.Message
                .Contains("duplicate key", StringComparison.OrdinalIgnoreCase) ?? false;
        }
    }
}
