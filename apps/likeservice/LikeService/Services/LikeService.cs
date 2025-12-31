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
using Waggle.Contracts.Comment.Events;
using Waggle.Contracts.Comment.Extensions;
using Waggle.Contracts.Comment.Interfaces;
using Waggle.Contracts.Like.Events;
using Waggle.Contracts.Post.Events;
using Waggle.Contracts.Post.Extensions;
using Waggle.Contracts.Post.Interfaces;
using Waggle.LikeService.Constants;
using Waggle.LikeService.Data;
using Waggle.LikeService.Dtos;
using Waggle.LikeService.Logging;
using Waggle.LikeService.Models;

namespace Waggle.LikeService.Services
{
    public class LikeService : ILikeService
    {
        private readonly ILikeRepository _repo;
        private readonly IMapper _mapper;
        private readonly IPostDataClient _postDataClient;
        private readonly ICommentDataClient _commentDataClient;
        private readonly IEventPublisher _eventPublisher;
        private readonly IServiceValidator _validator;
        private readonly ILogger<LikeService> _logger;

        public LikeService(
            ILikeRepository repo,
            IMapper mapper,
            IPostDataClient postDataClient,
            ICommentDataClient commentDataClient,
            IEventPublisher eventPublisher,
            IServiceValidator validator,
            ILogger<LikeService> logger)
        {
            _repo = repo;
            _mapper = mapper;
            _postDataClient = postDataClient;
            _commentDataClient = commentDataClient;
            _eventPublisher = eventPublisher;
            _validator = validator;
            _logger = logger;
        }

        public async Task<Result<PagedResult<LikeDto>>> GetLikesAsync(PaginationRequest request)
        {
            try
            {
                var likes = await _repo.GetLikesAsync(request: request);
                var dtos = _mapper.Map<List<LikeDto>>(likes.Items);

                var pagedResult = new PagedResult<LikeDto>
                {
                    Items = dtos,
                    PageInfo = likes.PageInfo
                };

                _logger.LogLikesRetrieved(dtos.Count);
                return Result<PagedResult<LikeDto>>.Ok(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogLikesRetrievalFailed(ex);
                return Result<PagedResult<LikeDto>>.Fail(LikeErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }

        public async Task<Result<LikeDto>> GetLikeByIdAsync(Guid id)
        {
            try
            {
                var like = await _repo.GetLikeByIdAsync(id);
                if (like == null)
                {
                    _logger.LogLikeNotFound(id);
                    return Result<LikeDto>.Fail(LikeErrors.Like.NotFound, ErrorCodes.NotFound);
                }

                _logger.LogLikeRetrieved(id);
                var dto = _mapper.Map<LikeDto>(like);
                return Result<LikeDto>.Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogLikeRetrievalFailed(ex, id);
                return Result<LikeDto>.Fail(LikeErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }

        public async Task<Result<PagedResult<LikeDto>>> GetLikesByTargetAsync(Guid targetId, PaginationRequest request)
        {
            try
            {
                var likes = await _repo.GetLikesAsync(targetId: targetId, request: request);
                var dtos = _mapper.Map<List<LikeDto>>(likes.Items);

                var pagedResult = new PagedResult<LikeDto>
                {
                    Items = dtos,
                    PageInfo = likes.PageInfo
                };

                _logger.LogLikesRetrieved(dtos.Count);
                return Result<PagedResult<LikeDto>>.Ok(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogLikesRetrievalFailed(ex);
                return Result<PagedResult<LikeDto>>.Fail(LikeErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }

        public async Task<Result<PagedResult<LikeDto>>> GetLikesByUserIdAsync(Guid userId, PaginationRequest request)
        {
            try
            {
                var likes = await _repo.GetLikesAsync(userId: userId, request: request);
                var dtos = _mapper.Map<List<LikeDto>>(likes.Items);

                var pagedResult = new PagedResult<LikeDto>
                {
                    Items = dtos,
                    PageInfo = likes.PageInfo
                };

                _logger.LogLikesRetrieved(dtos.Count);
                return Result<PagedResult<LikeDto>>.Ok(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogLikesRetrievalFailed(ex);
                return Result<PagedResult<LikeDto>>.Fail(LikeErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }

        public async Task<Result<Dictionary<Guid, int>>> GetLikeCountsAsync(IEnumerable<Guid> targetIds)
        {
            try
            {
                var counts = await _repo.GetLikeCountsAsync(targetIds);
                return Result<Dictionary<Guid,int>>.Ok(counts);
            }
            catch (Exception)
            {
                return Result<Dictionary<Guid,int>>.Fail(LikeErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }

        public async Task<Result<LikeDto>> HasLikedAsync(Guid userId, Guid targetId)
        {
            try
            {
                var like = await _repo.GetLikeByUserAndTargetAsync(userId, targetId);
                if (like == null)
                    return Result<LikeDto>.Ok(null!);

                var dto = _mapper.Map<LikeDto>(like);
                return Result<LikeDto>.Ok(dto);
            }
            catch (Exception)
            {
                return Result<LikeDto>.Fail(LikeErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }

        public async Task<Result<LikeDto>> CreateLikeAsync(LikeCreateDto request, UserInfoDto currentUser)
        {
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.Success)
                return Result<LikeDto>.ValidationFail(validationResult.ValidationErrors!);

            if (!currentUser.TryEnsure<LikeDto>(out var failedResult))
                return failedResult;

            var userId = currentUser.GetUserId();

            try
            {
                var like = _mapper.Map<Like>(request);

                var existingLike = await _repo.GetLikeByUserAndTargetAsync(userId, like.TargetId);
                if (existingLike != null)
                {
                    _logger.LogLikeAlreadyExists(existingLike.Id, userId);
                    return Result<LikeDto>.Fail(LikeErrors.Like.AlreadyExists, ErrorCodes.AlreadyExists);
                }

                switch (like.TargetType)
                {
                    case InteractionType.Post:
                        var postExists = await _postDataClient.GetPostByIdAsync(like.TargetId);
                        if (!postExists.Success || postExists.Data == null)
                        {
                            _logger.LogLikeTargetNotFound("Post", like.TargetId);
                            return Result<LikeDto>.Fail(LikeErrors.Like.TargetNotFound, ErrorCodes.NotFound);
                        }
                        break;

                    case InteractionType.Comment:
                        var commentExists = await _commentDataClient.GetCommentByIdAsync(like.TargetId);
                        if (!commentExists.Success || commentExists.Data == null)
                        {
                            _logger.LogLikeTargetNotFound("Comment", like.TargetId);
                            return Result<LikeDto>.Fail(LikeErrors.Like.TargetNotFound, ErrorCodes.NotFound);
                        }
                        break;
                }

                like.UserId = userId;
                like.CreatedAt = DateTime.UtcNow;

                await _repo.AddLikeAsync(like);

                var createdEvent = _mapper.Map<LikeCreatedEvent>(like);
                await _eventPublisher.PublishAsync(createdEvent);

                _logger.LogLikeCreated(like.Id, userId);

                return Result<LikeDto>.Ok(_mapper.Map<LikeDto>(like));
            }
            catch (DbUpdateException ex)
            {
                _logger.LogDatabaseUpdateFailed(ex, userId);
                return Result<LikeDto>.Fail(LikeErrors.Like.CreationFailed, ErrorCodes.ServiceFailed);
            }
            catch (Exception ex)
            {
                _logger.LogLikeCreationFailed(ex, userId);
                return Result<LikeDto>.Fail(LikeErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }

        public async Task<Result> DeleteLikeAsync(Guid id, UserInfoDto currentUser)
        {
            if (!currentUser.TryEnsure(out var failedResult))
                return failedResult;

            try
            {
                var like = await _repo.GetLikeByIdAsync(id);
                if (like == null)
                {
                    _logger.LogLikeDeleteNotFound(id);
                    return Result.Fail(LikeErrors.Like.NotFound, ErrorCodes.NotFound);
                }

                bool isAdmin = currentUser.IsAdmin();
                bool isSelf = currentUser.IsSelf(like.UserId);

                if (!isSelf && !isAdmin)
                {
                    _logger.LogUnauthorizedDeleteAttempt(currentUser.GetUserId(), id);
                    return Result.Fail(ErrorMessages.Authentication.PermissionRequired, ErrorCodes.Forbidden);
                }

                await _repo.DeleteLikeAsync(like);

                var deletedEvent = _mapper.Map<LikeDeletedEvent>(like);
                await _eventPublisher.PublishAsync(deletedEvent);

                _logger.LogLikeDeleted(id);
                return Result.Ok();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogLikeDeletionFailed(ex, id);
                return Result.Fail(LikeErrors.Like.DeletionFailed, ErrorCodes.ServiceFailed);
            }
            catch (Exception ex)
            {
                _logger.LogLikeDeletionFailed(ex, id);
                return Result.Fail(LikeErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }

        public async Task<Result> HandleCommentDeletedEventAsync(CommentDeletedEvent @event)
        {
            try
            {
                await _repo.DeleteLikesAsync(targetId: @event.Id);

                _logger.LogLikeDeletedFromEvent(@event.Id);
                return Result.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogLikeDeletionFromEventFailed(ex, @event.Id);
                return Result.Fail(LikeErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }

        public async Task<Result> HandlePostDeletedEventAsync(PostDeletedEvent @event)
        {
            try
            {
                await _repo.DeleteLikesAsync(targetId: @event.Id);

                _logger.LogLikeDeletedFromEvent(@event.Id);
                return Result.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogLikeDeletionFromEventFailed(ex, @event.Id);
                return Result.Fail(LikeErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }

        public async Task<Result> HandleUserDeletedEventAsync(UserDeletedEvent @event)
        {
            try
            {
                await _repo.DeleteLikesAsync(userId: @event.Id);

                _logger.LogLikeDeletedFromEvent(@event.Id);
                return Result.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogLikeDeletionFromEventFailed(ex, @event.Id);
                return Result.Fail(LikeErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }
    }
}