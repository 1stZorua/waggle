using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Waggle.Common.Auth;
using Waggle.Common.Constants;
using Waggle.Common.Messaging;
using Waggle.Common.Pagination.Models;
using Waggle.Common.Results.Core;
using Waggle.Contracts.Auth.Events;
using Waggle.Contracts.Follow.Events;
using Waggle.Contracts.User.Extensions;
using Waggle.Contracts.User.Interfaces;
using Waggle.FollowService.Constants;
using Waggle.FollowService.Data;
using Waggle.FollowService.Dtos;
using Waggle.FollowService.Logging;
using Waggle.FollowService.Models;

namespace Waggle.FollowService.Services
{
    public class FollowService : IFollowService
    {
        private readonly IFollowRepository _repo;
        private readonly IMapper _mapper;
        private readonly IUserDataClient _userDataClient;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILogger<FollowService> _logger;

        public FollowService(
            IFollowRepository repo,
            IMapper mapper,
            IUserDataClient userDataClient,
            IEventPublisher eventPublisher,
            ILogger<FollowService> logger)
        {
            _repo = repo;
            _mapper = mapper;
            _userDataClient = userDataClient;
            _eventPublisher = eventPublisher;
            _logger = logger;
        }

        public async Task<Result<PagedResult<FollowDto>>> GetFollowsAsync(PaginationRequest request)
        {
            try
            {
                var follows = await _repo.GetFollowsAsync(request: request);
                var dtos = _mapper.Map<List<FollowDto>>(follows.Items);

                var pagedResult = new PagedResult<FollowDto>
                {
                    Items = dtos,
                    PageInfo = follows.PageInfo
                };

                _logger.LogFollowsRetrieved(dtos.Count);
                return Result<PagedResult<FollowDto>>.Ok(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogFollowsRetrievalFailed(ex);
                return Result<PagedResult<FollowDto>>.Fail(FollowErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }

        public async Task<Result<FollowDto>> GetFollowByIdAsync(Guid id)
        {
            try
            {
                var follow = await _repo.GetFollowByIdAsync(id);
                if (follow == null)
                {
                    _logger.LogFollowNotFound(id);
                    return Result<FollowDto>.Fail(FollowErrors.Follow.NotFound, ErrorCodes.NotFound);
                }

                _logger.LogFollowRetrieved(id);
                var dto = _mapper.Map<FollowDto>(follow);
                return Result<FollowDto>.Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogFollowRetrievalFailed(ex, id);
                return Result<FollowDto>.Fail(FollowErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }

        public async Task<Result<PagedResult<FollowDto>>> GetFollowersAsync(Guid userId, PaginationRequest request)
        {
            try
            {
                var follows = await _repo.GetFollowsAsync(followingId: userId, request: request);
                var dtos = _mapper.Map<List<FollowDto>>(follows.Items);

                var pagedResult = new PagedResult<FollowDto>
                {
                    Items = dtos,
                    PageInfo = follows.PageInfo
                };

                _logger.LogFollowsRetrieved(dtos.Count);
                return Result<PagedResult<FollowDto>>.Ok(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogFollowsRetrievalFailed(ex);
                return Result<PagedResult<FollowDto>>.Fail(FollowErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }

        public async Task<Result<PagedResult<FollowDto>>> GetFollowingAsync(Guid userId, PaginationRequest request)
        {
            try
            {
                var follows = await _repo.GetFollowsAsync(followerId: userId, request: request);
                var dtos = _mapper.Map<List<FollowDto>>(follows.Items);

                var pagedResult = new PagedResult<FollowDto>
                {
                    Items = dtos,
                    PageInfo = follows.PageInfo
                };

                _logger.LogFollowsRetrieved(dtos.Count);
                return Result<PagedResult<FollowDto>>.Ok(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogFollowsRetrievalFailed(ex);
                return Result<PagedResult<FollowDto>>.Fail(FollowErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }

        public async Task<Result<Dictionary<Guid, int>>> GetFollowerCountsAsync(IEnumerable<Guid> userIds)
        {
            try
            {
                var counts = await _repo.GetFollowerCountsAsync(userIds);
                return Result<Dictionary<Guid, int>>.Ok(counts);
            }
            catch (Exception ex)
            {
                _logger.LogFollowsRetrievalFailed(ex);
                return Result<Dictionary<Guid, int>>.Fail(FollowErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }

        public async Task<Result<Dictionary<Guid, int>>> GetFollowingCountsAsync(IEnumerable<Guid> userIds)
        {
            try
            {
                var counts = await _repo.GetFollowingCountsAsync(userIds);
                return Result<Dictionary<Guid, int>>.Ok(counts);
            }
            catch (Exception ex)
            {
                _logger.LogFollowsRetrievalFailed(ex);
                return Result<Dictionary<Guid, int>>.Fail(FollowErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }

        public async Task<Result<FollowDto>> IsFollowingAsync(Guid followerId, Guid followingId)
        {
            try
            {
                var follow = await _repo.GetFollowByFollowerAndFollowingAsync(followerId, followingId);
                if (follow == null)
                    return Result<FollowDto>.Ok(null!);

                var dto = _mapper.Map<FollowDto>(follow);
                return Result<FollowDto>.Ok(dto);
            }
            catch (Exception)
            {
                return Result<FollowDto>.Fail(FollowErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }

        public async Task<Result<FollowDto>> CreateFollowAsync(FollowCreateDto request, UserInfoDto currentUser)
        {
            if (!currentUser.TryEnsure<FollowDto>(out var failedResult))
                return failedResult;

            var userId = currentUser.GetUserId();
            var isSelf = currentUser.IsSelf(request.FollowingId);

            if (isSelf)
            {
                _logger.LogCannotFollowSelf(userId);
                return Result<FollowDto>.Fail(FollowErrors.Follow.CannotFollowSelf, ErrorCodes.InvalidInput);
            }

            try
            {
                var existingFollow = await _repo.GetFollowByFollowerAndFollowingAsync(userId, request.FollowingId);
                if (existingFollow != null)
                {
                    _logger.LogFollowAlreadyExists(existingFollow.Id, userId);
                    return Result<FollowDto>.Fail(FollowErrors.Follow.AlreadyExists, ErrorCodes.AlreadyExists);
                }

                var userExists = await _userDataClient.GetUserByIdAsync(request.FollowingId);
                if (!userExists.Success || userExists.Data == null)
                {
                    _logger.LogFollowUserNotFound(request.FollowingId);
                    return Result<FollowDto>.Fail(FollowErrors.Follow.UserNotFound, ErrorCodes.NotFound);
                }

                var follow = new Follow
                {
                    Id = Guid.NewGuid(),
                    FollowerId = userId,
                    FollowingId = request.FollowingId,
                    CreatedAt = DateTime.UtcNow
                };

                await _repo.AddFollowAsync(follow);

                var createdEvent = _mapper.Map<FollowCreatedEvent>(follow);
                await _eventPublisher.PublishAsync(createdEvent);

                _logger.LogFollowCreated(follow.Id, userId, request.FollowingId);

                return Result<FollowDto>.Ok(_mapper.Map<FollowDto>(follow));
            }
            catch (DbUpdateException ex)
            {
                _logger.LogDatabaseUpdateFailed(ex, userId);
                return Result<FollowDto>.Fail(FollowErrors.Follow.CreationFailed, ErrorCodes.ServiceFailed);
            }
            catch (Exception ex)
            {
                _logger.LogFollowCreationFailed(ex, userId);
                return Result<FollowDto>.Fail(FollowErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }

        public async Task<Result> DeleteFollowAsync(Guid id, UserInfoDto currentUser)
        {
            if (!currentUser.TryEnsure(out var failedResult))
                return failedResult;

            try
            {
                var follow = await _repo.GetFollowByIdAsync(id);
                if (follow == null)
                {
                    _logger.LogFollowDeleteNotFound(id);
                    return Result.Fail(FollowErrors.Follow.NotFound, ErrorCodes.NotFound);
                }

                bool isAdmin = currentUser.IsAdmin();
                bool isSelf = currentUser.IsSelf(follow.FollowerId);

                if (!isSelf && !isAdmin)
                {
                    _logger.LogUnauthorizedDeleteAttempt(currentUser.GetUserId(), id);
                    return Result.Fail(ErrorMessages.Authentication.PermissionRequired, ErrorCodes.Forbidden);
                }

                await _repo.DeleteFollowAsync(follow);

                var deletedEvent = _mapper.Map<FollowDeletedEvent>(follow);
                await _eventPublisher.PublishAsync(deletedEvent);

                _logger.LogFollowDeleted(id);
                return Result.Ok();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogFollowDeletionFailed(ex, id);
                return Result.Fail(FollowErrors.Follow.DeletionFailed, ErrorCodes.ServiceFailed);
            }
            catch (Exception ex)
            {
                _logger.LogFollowDeletionFailed(ex, id);
                return Result.Fail(FollowErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }

        public async Task<Result> HandleUserDeletedEventAsync(UserDeletedEvent @event)
        {
            try
            {
                await _repo.DeleteFollowsAsync(followerId: @event.Id);
                await _repo.DeleteFollowsAsync(followingId: @event.Id);
                return Result.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogFollowDeletionFromEventFailed(ex, @event.Id);
                return Result.Fail(FollowErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }
    }
}