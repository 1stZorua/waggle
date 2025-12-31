using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Waggle.Common.Auth;
using Waggle.Common.Constants;
using Waggle.Common.Messaging;
using Waggle.Common.Pagination.Models;
using Waggle.Common.Results.Core;
using Waggle.Common.Validation;
using Waggle.Contracts.Auth.Events;
using Waggle.Contracts.Follow.Extensions;
using Waggle.Contracts.Follow.Interfaces;
using Waggle.Contracts.Media.Grpc;
using Waggle.Contracts.Media.Interfaces;
using Waggle.Contracts.Post.Extensions;
using Waggle.Contracts.Post.Interfaces;
using Waggle.Contracts.User.Events;
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
        private readonly IMediaDataClient _mediaDataClient;
        private readonly IPostDataClient _postDataClient;
        private readonly IFollowDataClient _followDataClient;
        private readonly IEventPublisher _eventPublisher;
        private readonly IServiceValidator _validator;
        private readonly ILogger<UserService> _logger;

        public UserService(
            IUserRepository repo,
            IMapper mapper,
            IMediaDataClient mediaDataClient,
            IPostDataClient postDataClient,
            IFollowDataClient followDataClient,
            IEventPublisher eventPublisher,
            IServiceValidator serviceValidator,
            ILogger<UserService> logger)
        {
            _repo = repo;
            _mapper = mapper;
            _mediaDataClient = mediaDataClient;
            _postDataClient = postDataClient;
            _followDataClient = followDataClient;
            _eventPublisher = eventPublisher;
            _validator = serviceValidator;
            _logger = logger;
        }

        public async Task<Result<PagedResult<UserDto>>> GetUsersAsync(PaginationRequest request)
        {
            try
            {
                var users = await _repo.GetUsersAsync(request);
                var dtos = await EnrichUsersWithMetadataAsync(users.Items);

                var pagedResult = new PagedResult<UserDto>
                {
                    Items = dtos,
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

                var dto = await EnrichUserWithMetadataAsync(user);
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
                var dtos = await EnrichUsersWithMetadataAsync(users);

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

                var result = await EnrichUserWithMetadataAsync(user);
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

        public async Task<Result<UserDto>> UpdateUserAsync(Guid id, UserUpdateDto request, UserInfoDto currentUser)
        {
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.Success)
                return Result<UserDto>.ValidationFail(validationResult.ValidationErrors!);

            if (!currentUser.TryEnsure<UserDto>(out var failedResult))
                return failedResult;

            var userId = currentUser.GetUserId();

            try
            {
                var user = await _repo.GetUserByIdAsync(id);
                if (user == null)
                {
                    _logger.LogUserNotFound(id);
                    return Result<UserDto>.Fail(UserErrors.User.NotFound, ErrorCodes.NotFound);
                }

                bool isAdmin = currentUser.IsAdmin();
                bool isSelf = currentUser.IsSelf(id);

                if (!isSelf && !isAdmin)
                {
                    _logger.LogUnauthorizedUserAccess(id, userId);
                    return Result<UserDto>.Fail(ErrorMessages.Authentication.PermissionRequired, ErrorCodes.Forbidden);
                }

                var mediaUrlsResult = await _mediaDataClient.GetMediaUrlsAsync(
                    new GetMediaUrlsRequest { Ids = { request.AvatarId.ToString() } }
                );

                if (!mediaUrlsResult.Success || mediaUrlsResult.Data?.Urls.Count != 1)
                {
                    _logger.LogUserUpdateFailedMedia(userId);
                    return Result<UserDto>.Fail(UserErrors.User.MediaDoesNotExist, ErrorCodes.InvalidInput);
                }

                user.AvatarId = request.AvatarId;
                user.UpdatedAt = DateTime.UtcNow;

                await _repo.UpdateUserAsync(user);

                var updatedEvent = _mapper.Map<UserUpdatedEvent>(user);
                await _eventPublisher.PublishAsync(updatedEvent);

                var dto = await EnrichUserWithMetadataAsync(user);

                _logger.LogUserUpdated(user.Id);
                return Result<UserDto>.Ok(dto);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogDatabaseUpdateFailed(ex, userId);
                return Result<UserDto>.Fail(UserErrors.User.UpdateFailed, ErrorCodes.ServiceFailed);
            }
            catch (Exception ex)
            {
                _logger.LogUserUpdateFailed(ex, id, userId);
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

        public async Task<Result<UserDto>> HandleUserRegisteredEventAsync(RegisteredEvent @event)
        {
            try
            {
                var existing = await _repo.GetUserByIdAsync(@event.Id);
                if (existing != null)
                {
                    _logger.LogUserExistsFromEvent(@event.Id);
                    var existingDto = await EnrichUserWithMetadataAsync(existing);
                    return Result<UserDto>.Ok(existingDto);
                }

                var createDto = _mapper.Map<UserCreateDto>(@event);

                var user = _mapper.Map<User>(createDto);
                user.CreatedAt = DateTime.UtcNow;
                user.UpdatedAt = DateTime.UtcNow;

                await _repo.AddUserAsync(user);

                _logger.LogUserCreatedFromEvent(user.Username, user.Id);

                var result = await EnrichUserWithMetadataAsync(user);
                return Result<UserDto>.Ok(result);
            }
            catch (DbUpdateException ex) when (IsDuplicateKeyError(ex))
            {
                _logger.LogDuplicateKeyErrorFromEvent(ex, @event.Id);

                var existing = await _repo.GetUserByIdAsync(@event.Id);
                if (existing != null)
                {
                    var existingDto = await EnrichUserWithMetadataAsync(existing);
                    return Result<UserDto>.Ok(existingDto);
                }

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

        public async Task<Result> HandleUserDeletedEventAsync(UserDeletedEvent @event)
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

        private async Task<List<UserDto>> EnrichUsersWithMetadataAsync(List<User> users)
        {
            if (users.Count == 0) return [];

            var userIds = users.Select(u => u.Id).ToList();
            var avatarIds = users.Select(u => u.AvatarId).Distinct().ToList();

            var mediaUrlsTask = _mediaDataClient.GetMediaUrlsAsync(
                new GetMediaUrlsRequest { Ids = { avatarIds.Select(id => id.ToString()) } }
            );
            var postCountsTask = _postDataClient.GetPostCountsAsync(userIds);
            var followerCountsTask = _followDataClient.GetFollowerCountsAsync(userIds);
            var followingCountsTask = _followDataClient.GetFollowingCountsAsync(userIds);

            await Task.WhenAll(mediaUrlsTask, postCountsTask, followerCountsTask, followingCountsTask);

            var mediaUrlsResult = await mediaUrlsTask;
            var postCountsResult = await postCountsTask;
            var followerCountsResult = await followerCountsTask;
            var followingCountsResult = await followingCountsTask;

            var avatarUrls = mediaUrlsResult.Success && mediaUrlsResult.Data != null
                ? mediaUrlsResult.Data.Urls.ToDictionary(
                    kvp => Guid.Parse(kvp.Key),
                    kvp => new UrlResponseDto
                    {
                        Url = kvp.Value.Url,
                        ExpiresAt = kvp.Value.ExpiresAt.ToDateTime()
                    }
                )
                : [];

            var postCounts = postCountsResult.Success && postCountsResult.Data?.Counts != null
                ? postCountsResult.Data.Counts.ToDictionary(x => Guid.Parse(x.UserId), x => x.Count)
                : [];

            var followerCounts = followerCountsResult.Success && followerCountsResult.Data?.Counts != null
                ? followerCountsResult.Data.Counts.ToDictionary(x => Guid.Parse(x.UserId), x => x.Count)
                : [];

            var followingCounts = followingCountsResult.Success && followingCountsResult.Data?.Counts != null
                ? followingCountsResult.Data.Counts.ToDictionary(x => Guid.Parse(x.UserId), x => x.Count)
                : [];

            return [.. users.Select(user =>
            {
                var dto = _mapper.Map<UserDto>(user);
                dto.AvatarUrl = avatarUrls.GetValueOrDefault(user.AvatarId);
                dto.PostCount = postCounts.TryGetValue(user.Id, out var postCount) ? postCount : 0;
                dto.FollowerCount = followerCounts.TryGetValue(user.Id, out var followerCount) ? followerCount : 0;
                dto.FollowingCount = followingCounts.TryGetValue(user.Id, out var followingCount) ? followingCount : 0;
                return dto;
            })];
        }

        private async Task<UserDto> EnrichUserWithMetadataAsync(User user)
        {
            var users = new List<User> { user };
            var dtos = await EnrichUsersWithMetadataAsync(users);
            return dtos.First();
        }

        private static bool IsDuplicateKeyError(DbUpdateException ex)
        {
            return ex.InnerException?.Message
                .Contains("duplicate key", StringComparison.OrdinalIgnoreCase) ?? false;
        }
    }
}