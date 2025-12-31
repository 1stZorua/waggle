using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Waggle.Common.Auth;
using Waggle.Common.Constants;
using Waggle.Common.Messaging;
using Waggle.Common.Models;
using Waggle.Common.Pagination.Models;
using Waggle.Common.Results.Core;
using Waggle.Common.Validation;
using Waggle.Contracts.Auth.Events;
using Waggle.Contracts.Favorite.Events;
using Waggle.Contracts.Post.Events;
using Waggle.Contracts.Post.Extensions;
using Waggle.Contracts.Post.Interfaces;
using Waggle.FavoriteService.Constants;
using Waggle.FavoriteService.Dtos;
using Waggle.FavoriteService.Logging;
using Waggle.FavoriteService.Models;
using Waggle.FollowService.Data;

namespace Waggle.FavoriteService.Services
{
    public class FavoriteService : IFavoriteService
    {
        private readonly IFavoriteRepository _repo;
        private readonly IMapper _mapper;
        private readonly IPostDataClient _postDataClient;
        private readonly IEventPublisher _eventPublisher;
        private readonly IServiceValidator _validator;
        private readonly ILogger<FavoriteService> _logger;

        public FavoriteService(
            IFavoriteRepository repo,
            IMapper mapper,
            IPostDataClient postDataClient,
            IEventPublisher eventPublisher,
            IServiceValidator validator,
            ILogger<FavoriteService> logger)
        {
            _repo = repo;
            _mapper = mapper;
            _postDataClient = postDataClient;
            _eventPublisher = eventPublisher;
            _validator = validator;
            _logger = logger;
        }

        public async Task<Result<PagedResult<FavoriteDto>>> GetFavoritesAsync(PaginationRequest request)
        {
            try
            {
                var favorites = await _repo.GetFavoritesAsync(request: request);
                var dtos = _mapper.Map<List<FavoriteDto>>(favorites.Items);

                var pagedResult = new PagedResult<FavoriteDto>
                {
                    Items = dtos,
                    PageInfo = favorites.PageInfo
                };

                _logger.LogFavoritesRetrieved(dtos.Count);
                return Result<PagedResult<FavoriteDto>>.Ok(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogFavoritesRetrievalFailed(ex);
                return Result<PagedResult<FavoriteDto>>.Fail(FavoriteErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }

        public async Task<Result<FavoriteDto>> GetFavoriteByIdAsync(Guid id)
        {
            try
            {
                var favorite = await _repo.GetFavoriteByIdAsync(id);
                if (favorite == null)
                {
                    _logger.LogFavoriteNotFound(id);
                    return Result<FavoriteDto>.Fail(FavoriteErrors.Favorite.NotFound, ErrorCodes.NotFound);
                }

                _logger.LogFavoriteRetrieved(id);
                var dto = _mapper.Map<FavoriteDto>(favorite);
                return Result<FavoriteDto>.Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogFavoriteRetrievalFailed(ex, id);
                return Result<FavoriteDto>.Fail(FavoriteErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }

        public async Task<Result<PagedResult<FavoriteDto>>> GetFavoritesByTargetAsync(
            Guid targetId,
            PaginationRequest request)
        {
            try
            {
                var favorites = await _repo.GetFavoritesAsync(
                    targetId: targetId,
                    request: request);
                var dtos = _mapper.Map<List<FavoriteDto>>(favorites.Items);

                var pagedResult = new PagedResult<FavoriteDto>
                {
                    Items = dtos,
                    PageInfo = favorites.PageInfo
                };

                _logger.LogFavoritesRetrieved(dtos.Count);
                return Result<PagedResult<FavoriteDto>>.Ok(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogFavoritesRetrievalFailed(ex);
                return Result<PagedResult<FavoriteDto>>.Fail(FavoriteErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }

        public async Task<Result<PagedResult<FavoriteDto>>> GetFavoritesByUserIdAsync(Guid userId, PaginationRequest request)
        {
            try
            {
                var favorites = await _repo.GetFavoritesAsync(userId: userId, request: request);
                var dtos = _mapper.Map<List<FavoriteDto>>(favorites.Items);

                var pagedResult = new PagedResult<FavoriteDto>
                {
                    Items = dtos,
                    PageInfo = favorites.PageInfo
                };

                _logger.LogFavoritesRetrieved(dtos.Count);
                return Result<PagedResult<FavoriteDto>>.Ok(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogFavoritesRetrievalFailed(ex);
                return Result<PagedResult<FavoriteDto>>.Fail(FavoriteErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }

        public async Task<Result<Dictionary<Guid, int>>> GetFavoriteCountsAsync(IEnumerable<Guid> targetIds)
        {
            try
            {
                var counts = await _repo.GetFavoriteCountsAsync(targetIds);
                return Result<Dictionary<Guid, int>>.Ok(counts);
            }
            catch (Exception ex)
            {
                _logger.LogFavoritesRetrievalFailed(ex);
                return Result<Dictionary<Guid, int>>.Fail(FavoriteErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }

        public async Task<Result<FavoriteDto>> HasFavoritedAsync(Guid userId, Guid targetId)
        {
            try
            {
                var favorite = await _repo.GetFavoriteByUserAndTargetAsync(userId, targetId);
                if (favorite == null)
                    return Result<FavoriteDto>.Ok(null!);

                var dto = _mapper.Map<FavoriteDto>(favorite);
                return Result<FavoriteDto>.Ok(dto);
            }
            catch (Exception)
            {
                return Result<FavoriteDto>.Fail(FavoriteErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }

        public async Task<Result<FavoriteDto>> CreateFavoriteAsync(FavoriteCreateDto request, UserInfoDto currentUser)
        {
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.Success)
                return Result<FavoriteDto>.ValidationFail(validationResult.ValidationErrors!);

            if (!currentUser.TryEnsure<FavoriteDto>(out var failedResult))
                return failedResult;

            var userId = currentUser.GetUserId();

            try
            {
                var favorite = _mapper.Map<Favorite>(request);

                var existingFavorite = await _repo.GetFavoriteByUserAndTargetAsync(userId, favorite.TargetId);
                if (existingFavorite != null)
                {
                    _logger.LogFavoriteAlreadyExists(existingFavorite.Id, userId);
                    return Result<FavoriteDto>.Fail(FavoriteErrors.Favorite.AlreadyExists, ErrorCodes.AlreadyExists);
                }

                if (favorite.TargetType != InteractionType.Post)
                {
                    return Result<FavoriteDto>.Fail(FavoriteErrors.Favorite.TargetNotFound, ErrorCodes.NotFound);
                }

                var postExists = await _postDataClient.GetPostByIdAsync(favorite.TargetId);
                if (!postExists.Success || postExists.Data == null)
                {
                    _logger.LogFavoriteTargetNotFound("Post", favorite.TargetId);
                    return Result<FavoriteDto>.Fail(FavoriteErrors.Favorite.TargetNotFound, ErrorCodes.NotFound);
                }

                favorite.UserId = userId;
                favorite.CreatedAt = DateTime.UtcNow;

                await _repo.AddFavoriteAsync(favorite);

                var createdEvent = _mapper.Map<FavoriteCreatedEvent>(favorite);
                await _eventPublisher.PublishAsync(createdEvent);

                _logger.LogFavoriteCreated(favorite.Id, userId);

                return Result<FavoriteDto>.Ok(_mapper.Map<FavoriteDto>(favorite));
            }
            catch (DbUpdateException ex)
            {
                _logger.LogDatabaseUpdateFailed(ex, userId);
                return Result<FavoriteDto>.Fail(FavoriteErrors.Favorite.CreationFailed, ErrorCodes.ServiceFailed);
            }
            catch (Exception ex)
            {
                _logger.LogFavoriteCreationFailed(ex, userId);
                return Result<FavoriteDto>.Fail(FavoriteErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }

        public async Task<Result> DeleteFavoriteAsync(Guid id, UserInfoDto currentUser)
        {
            if (!currentUser.TryEnsure(out var failedResult))
                return failedResult;

            try
            {
                var favorite = await _repo.GetFavoriteByIdAsync(id);
                if (favorite == null)
                {
                    _logger.LogFavoriteDeleteNotFound(id);
                    return Result.Fail(FavoriteErrors.Favorite.NotFound, ErrorCodes.NotFound);
                }

                bool isAdmin = currentUser.IsAdmin();
                bool isSelf = currentUser.IsSelf(favorite.UserId);

                if (!isSelf && !isAdmin)
                {
                    _logger.LogUnauthorizedDeleteAttempt(currentUser.GetUserId(), id);
                    return Result.Fail(ErrorMessages.Authentication.PermissionRequired, ErrorCodes.Forbidden);
                }

                await _repo.DeleteFavoriteAsync(favorite);

                var deletedEvent = _mapper.Map<FavoriteDeletedEvent>(favorite);
                await _eventPublisher.PublishAsync(deletedEvent);

                _logger.LogFavoriteDeleted(id);
                return Result.Ok();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogFavoriteDeletionFailed(ex, id);
                return Result.Fail(FavoriteErrors.Favorite.DeletionFailed, ErrorCodes.ServiceFailed);
            }
            catch (Exception ex)
            {
                _logger.LogFavoriteDeletionFailed(ex, id);
                return Result.Fail(FavoriteErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }

        public async Task<Result> HandlePostDeletedEventAsync(PostDeletedEvent @event)
        {
            try
            {
                await _repo.DeleteFavoritesAsync(targetId: @event.Id);
                _logger.LogFavoriteDeletedFromEvent(@event.Id);
                return Result.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogFavoriteDeletionFromEventFailed(ex, @event.Id);
                return Result.Fail(FavoriteErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }

        public async Task<Result> HandleUserDeletedEventAsync(UserDeletedEvent @event)
        {
            try
            {
                await _repo.DeleteFavoritesAsync(userId: @event.Id);
                _logger.LogFavoriteDeletedFromEvent(@event.Id);
                return Result.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogFavoriteDeletionFromEventFailed(ex, @event.Id);
                return Result.Fail(FavoriteErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }
    }
}