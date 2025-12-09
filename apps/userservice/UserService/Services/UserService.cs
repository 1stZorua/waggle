using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Waggle.Common.Constants;
using Waggle.Common.Pagination.Models;
using Waggle.Common.Results.Core;
using Waggle.Contracts.Auth.Events;
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
                var users = await _repo.GetAllUsersAsync(request);

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
                var user = await _repo.GetUserByIdAsync(id);
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

        public async Task<Result<List<UserDto>>> GetUsersByIdAsync(UserBatchRequest request)
        {
            try
            {
                var users = await _repo.GetUsersByIdAsync(request);
                
                var dtos = _mapper.Map<List<UserDto>>(users);
                return Result<List<UserDto>>.Ok(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogUsersBatchRetrievalFailed(ex);
                return Result<List<UserDto>>.Fail(UserErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }

        public async Task<Result<UserDto>> CreateUserAsync(UserCreateDto request)
        {
            try
            {
                var existing = await _repo.GetUserByIdAsync(request.Id);
                if (existing != null)
                {
                    _logger.LogUserAlreadyExists(request.Id);
                    return Result<UserDto>.Fail(UserErrors.User.AlreadyExists, ErrorCodes.AlreadyExists);
                }

                var user = _mapper.Map<User>(request);
                user.CreatedAt = DateTime.UtcNow;
                user.UpdatedAt = DateTime.UtcNow;

                await _repo.AddUserAsync(user);

                _logger.LogUserCreated(request.Username, request.Id);

                var result = _mapper.Map<UserDto>(user);
                return Result<UserDto>.Ok(result);
            }
            catch (DbUpdateException ex) when (IsDuplicateKeyError(ex))
            {
                _logger.LogDuplicateKeyError(ex, request.Id);
                return Result<UserDto>.Fail(UserErrors.User.AlreadyExists, ErrorCodes.AlreadyExists);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogDatabaseUpdateFailed(ex, request.Id);
                return Result<UserDto>.Fail(UserErrors.User.CreationFailed, ErrorCodes.ServiceFailed);
            }
            catch (Exception ex)
            {
                _logger.LogUserCreationFailed(ex, request.Id);
                return Result<UserDto>.Fail(UserErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }

        public async Task<Result> DeleteUserAsync(Guid id)
        {
            try
            {
                var existing = await _repo.GetUserByIdAsync(id);
                if (existing == null)
                {
                    _logger.LogUserDeleteNotFound(id);
                    return Result.Fail(UserErrors.User.NotFound, ErrorCodes.NotFound);
                }

                await _repo.DeleteUserAsync(existing);

                _logger.LogUserDeleted(id);
                return Result.Ok();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogUserDeletionFailed(ex, id);
                return Result.Fail(UserErrors.User.DeletionFailed, ErrorCodes.ServiceFailed);
            }
            catch (Exception ex)
            {
                _logger.LogUserDeletionFailed(ex, id);
                return Result.Fail(UserErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }

        public async Task<Result<UserDto>> HandleUserRegisteredAsync(RegisteredEvent @event)
        {
            try
            {
                var existing = await _repo.GetUserByIdAsync(@event.Id);
                if (existing != null)
                {
                    _logger.LogUserExistsFromEvent(@event.Id);
                    return Result<UserDto>.Ok(_mapper.Map<UserDto>(existing));
                }

                var createDto = _mapper.Map<UserCreateDto>(@event);
                
                var user = _mapper.Map<User>(createDto);
                user.CreatedAt = DateTime.UtcNow;
                user.UpdatedAt = DateTime.UtcNow;

                await _repo.AddUserAsync(user);

                _logger.LogUserCreatedFromEvent(user.Username, user.Id);

                var result = _mapper.Map<UserDto>(user);
                return Result<UserDto>.Ok(result);
            }
            catch (DbUpdateException ex) when (IsDuplicateKeyError(ex))
            {
                _logger.LogDuplicateKeyErrorFromEvent(ex, @event.Id);

                var existing = await _repo.GetUserByIdAsync(@event.Id);
                if (existing != null)
                    return Result<UserDto>.Ok(_mapper.Map<UserDto>(existing));

                return Result<UserDto>.Fail(
                    UserErrors.User.CreationFailed,
                    ErrorCodes.ServiceFailed);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogDatabaseUpdateFailedFromEvent(ex, @event.Id);
                return Result<UserDto>.Fail(UserErrors.User.CreationFailed, ErrorCodes.ServiceFailed);
            }
            catch (Exception ex)
            {
                _logger.LogUserCreationFromEventFailed(ex, @event.Id);
                return Result<UserDto>.Fail(UserErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }

        public async Task<Result> HandleUserDeletedAsync(DeletedEvent @event)
        {
            try
            {
                var existing = await _repo.GetUserByIdAsync(@event.Id);
                if (existing == null)
                {
                    _logger.LogUserDeleteNotFoundFromEvent(@event.Id);
                    return Result.Ok();
                }

                await _repo.DeleteUserAsync(existing);

                _logger.LogUserDeletedFromEvent(@event.Id);
                return Result.Ok();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogUserDeletionFromEventFailed(ex, @event.Id);
                return Result.Fail(UserErrors.User.DeletionFailed, ErrorCodes.ServiceFailed);
            }
            catch (Exception ex)
            {
                _logger.LogUserDeletionFromEventFailed(ex, @event.Id);
                return Result.Fail(UserErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }

        private bool IsDuplicateKeyError(DbUpdateException ex)
        {
            return ex.InnerException?.Message
                .Contains("duplicate key", StringComparison.OrdinalIgnoreCase) ?? false;
        }
    }
}
