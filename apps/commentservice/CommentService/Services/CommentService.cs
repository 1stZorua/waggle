using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Waggle.CommentService.Constants;
using Waggle.CommentService.Data;
using Waggle.CommentService.Dtos;
using Waggle.CommentService.Logging;
using Waggle.CommentService.Models;
using Waggle.CommentService.Saga.Context;
using Waggle.Common.Auth;
using Waggle.Common.Constants;
using Waggle.Common.Messaging;
using Waggle.Common.Pagination.Models;
using Waggle.Common.Results.Core;
using Waggle.Common.Saga;
using Waggle.Common.Validation;
using Waggle.Contracts.Auth.Events;
using Waggle.Contracts.Comment.Events;
using Waggle.Contracts.Like.Extensions;
using Waggle.Contracts.Like.Interfaces;
using Waggle.Contracts.Post.Events;
using Waggle.Contracts.Post.Extensions;
using Waggle.Contracts.Post.Interfaces;

namespace Waggle.CommentService.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _repo;
        private readonly IMapper _mapper;
        private readonly IPostDataClient _postDataClient;
        private readonly ILikeDataClient _likeDataClient;
        private readonly IEventPublisher _eventPublisher;
        private readonly IServiceValidator _validator;
        private readonly ISagaCoordinator<DeletionSagaContext> _deletionSaga;
        private readonly ILogger<CommentService> _logger;

        public CommentService(
            ICommentRepository repo,
            IMapper mapper,
            IPostDataClient postDataClient,
            ILikeDataClient likeDataClient,
            IEventPublisher eventPublisher,
            IServiceValidator validator,
            ISagaCoordinator<DeletionSagaContext> deletionSaga,
            ILogger<CommentService> logger)
        {
            _repo = repo;
            _mapper = mapper;
            _postDataClient = postDataClient;
            _likeDataClient = likeDataClient;
            _eventPublisher = eventPublisher;
            _validator = validator;
            _deletionSaga = deletionSaga;
            _logger = logger;
        }

        public async Task<Result<PagedResult<CommentDto>>> GetCommentsAsync(PaginationRequest request)
        {
            try
            {
                var comments = await _repo.GetCommentsAsync(request: request);
                var dtos = await EnrichCommentsWithMetadataAsync(comments.Items);

                var pagedResult = new PagedResult<CommentDto>
                {
                    Items = dtos,
                    PageInfo = comments.PageInfo
                };

                _logger.LogCommentsRetrieved(dtos.Count);
                return Result<PagedResult<CommentDto>>.Ok(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogCommentsRetrievalFailed(ex);
                return Result<PagedResult<CommentDto>>.Fail(CommentErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }

        public async Task<Result<CommentDto>> GetCommentByIdAsync(Guid id)
        {
            try
            {
                var comment = await _repo.GetCommentByIdAsync(id);
                if (comment == null)
                {
                    _logger.LogCommentNotFound(id);
                    return Result<CommentDto>.Fail(CommentErrors.Comment.NotFound, ErrorCodes.NotFound);
                }

                _logger.LogCommentRetrieved(id);
                var dto = await EnrichCommentWithMetadataAsync(comment);
                return Result<CommentDto>.Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogCommentRetrievalFailed(ex, id);
                return Result<CommentDto>.Fail(CommentErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }

        public async Task<Result<PagedResult<CommentDto>>> GetCommentsByPostAsync(Guid postId, PaginationRequest request)
        {
            try
            {
                var comments = await _repo.GetCommentsAsync(postId: postId, request: request);
                var dtos = await EnrichCommentsWithMetadataAsync(comments.Items);

                var pagedResult = new PagedResult<CommentDto>
                {
                    Items = dtos,
                    PageInfo = comments.PageInfo
                };

                _logger.LogCommentsRetrieved(dtos.Count);
                return Result<PagedResult<CommentDto>>.Ok(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogCommentsRetrievalFailed(ex);
                return Result<PagedResult<CommentDto>>.Fail(CommentErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }

        public async Task<Result<PagedResult<CommentDto>>> GetRepliesAsync(Guid commentId, PaginationRequest request)
        {
            try
            {
                var comments = await _repo.GetCommentsAsync(parentId: commentId, request: request);
                var dtos = await EnrichCommentsWithMetadataAsync(comments.Items);

                var pagedResult = new PagedResult<CommentDto>
                {
                    Items = dtos,
                    PageInfo = comments.PageInfo
                };

                _logger.LogCommentsRetrieved(dtos.Count);
                return Result<PagedResult<CommentDto>>.Ok(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogCommentsRetrievalFailed(ex);
                return Result<PagedResult<CommentDto>>.Fail(CommentErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }

        public async Task<Result<PagedResult<CommentDto>>> GetCommentsByUserAsync(Guid userId, PaginationRequest request)
        {
            try
            {
                var comments = await _repo.GetCommentsAsync(userId: userId, request: request);
                var dtos = await EnrichCommentsWithMetadataAsync(comments.Items);

                var pagedResult = new PagedResult<CommentDto>
                {
                    Items = dtos,
                    PageInfo = comments.PageInfo
                };

                _logger.LogCommentsRetrieved(dtos.Count);
                return Result<PagedResult<CommentDto>>.Ok(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogCommentsRetrievalFailed(ex);
                return Result<PagedResult<CommentDto>>.Fail(CommentErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }

        public async Task<Result<Dictionary<Guid, int>>> GetCommentCountsAsync(IEnumerable<Guid> postIds)
        {
            try
            {
                var counts = await _repo.GetCommentCountsAsync(postIds);
                return Result<Dictionary<Guid, int>>.Ok(counts);
            }
            catch (Exception ex)
            {
                _logger.LogCommentsRetrievalFailed(ex);
                return Result<Dictionary<Guid, int>>.Fail(CommentErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }

        public async Task<Result<Dictionary<Guid, int>>> GetReplyCountsAsync(IEnumerable<Guid> commentIds)
        {
            try
            {
                var counts = await _repo.GetReplyCountsAsync(commentIds);
                return Result<Dictionary<Guid, int>>.Ok(counts);
            }
            catch (Exception ex)
            {
                _logger.LogCommentsRetrievalFailed(ex);
                return Result<Dictionary<Guid, int>>.Fail(CommentErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }

        public async Task<Result<CommentDto>> CreateCommentAsync(CommentCreateDto request, UserInfoDto currentUser)
        {
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.Success)
                return Result<CommentDto>.ValidationFail(validationResult.ValidationErrors!);

            if (!currentUser.TryEnsure<CommentDto>(out var failedResult))
                return failedResult;

            var userId = currentUser.GetUserId();

            try
            {
                var postExists = await _postDataClient.GetPostByIdAsync(request.PostId);
                if (!postExists.Success || postExists.Data == null)
                {
                    _logger.LogCommentPostNotFound(request.PostId);
                    return Result<CommentDto>.Fail(CommentErrors.Comment.PostNotFound, ErrorCodes.NotFound);
                }

                if (request.ParentId.HasValue)
                {
                    var parentComment = await _repo.GetCommentByIdAsync(request.ParentId.Value);
                    if (parentComment == null)
                    {
                        _logger.LogCommentParentNotFound(request.ParentId.Value);
                        return Result<CommentDto>.Fail(CommentErrors.Comment.ParentCommentNotFound, ErrorCodes.NotFound);
                    }

                    if (parentComment.PostId != request.PostId)
                    {
                        _logger.LogCommentParentPostMismatch(request.ParentId.Value, request.PostId);
                        return Result<CommentDto>.Fail(CommentErrors.Comment.ParentCommentNotFound, ErrorCodes.InvalidInput);
                    }
                }

                var comment = _mapper.Map<Comment>(request);

                comment.UserId = userId;
                comment.CreatedAt = DateTime.UtcNow;
                comment.UpdatedAt = DateTime.UtcNow;

                await _repo.AddCommentAsync(comment);

                var createdEvent = _mapper.Map<CommentCreatedEvent>(comment);
                await _eventPublisher.PublishAsync(createdEvent);

                _logger.LogCommentCreated(comment.Id, userId);

                var dto = await EnrichCommentWithMetadataAsync(comment);
                return Result<CommentDto>.Ok(dto);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogDatabaseUpdateFailed(ex, userId);
                return Result<CommentDto>.Fail(CommentErrors.Comment.CreationFailed, ErrorCodes.ServiceFailed);
            }
            catch (Exception ex)
            {
                _logger.LogCommentCreationFailed(ex, userId);
                return Result<CommentDto>.Fail(CommentErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }

        public async Task<Result<CommentDto>> UpdateCommentAsync(Guid id, CommentUpdateDto request, UserInfoDto currentUser)
        {
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.Success)
                return Result<CommentDto>.ValidationFail(validationResult.ValidationErrors!);

            if (!currentUser.TryEnsure<CommentDto>(out var failedResult))
                return failedResult;

            try
            {
                var comment = await _repo.GetCommentByIdAsync(id);
                if (comment == null)
                {
                    _logger.LogCommentNotFound(id);
                    return Result<CommentDto>.Fail(CommentErrors.Comment.NotFound, ErrorCodes.NotFound);
                }

                if (comment.PostId != request.PostId)
                {
                    _logger.LogCommentPostMismatch(id, request.PostId);
                    return Result<CommentDto>.Fail(CommentErrors.Comment.NotFound, ErrorCodes.NotFound);
                }

                bool isAdmin = currentUser.IsAdmin();
                bool isSelf = currentUser.IsSelf(comment.UserId);

                if (!isSelf && !isAdmin)
                {
                    _logger.LogUnauthorizedUpdateAttempt(currentUser.GetUserId(), id);
                    return Result<CommentDto>.Fail(ErrorMessages.Authentication.PermissionRequired, ErrorCodes.Forbidden);
                }

                comment.Content = request.Content;
                comment.UpdatedAt = DateTime.UtcNow;

                await _repo.UpdateCommentAsync(comment);

                var updatedEvent = _mapper.Map<CommentUpdatedEvent>(comment);
                await _eventPublisher.PublishAsync(updatedEvent);

                _logger.LogCommentUpdated(id);

                var dto = await EnrichCommentWithMetadataAsync(comment);
                return Result<CommentDto>.Ok(dto);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogCommentUpdateFailed(ex, id);
                return Result<CommentDto>.Fail(CommentErrors.Comment.UpdateFailed, ErrorCodes.ServiceFailed);
            }
            catch (Exception ex)
            {
                _logger.LogCommentUpdateFailed(ex, id);
                return Result<CommentDto>.Fail(CommentErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }

        public async Task<Result> DeleteCommentAsync(Guid id, UserInfoDto currentUser)
        {
            if (!currentUser.TryEnsure(out var failedResult))
                return failedResult;

            var context = new DeletionSagaContext
            {
                Id = id,
                CurrentUser = currentUser
            };

            return await _deletionSaga.ExecuteAsync(context);
        }

        public async Task<Result> HandlePostDeletedEventAsync(PostDeletedEvent @event)
        {
            try
            {
                await _repo.DeleteCommentsAsync(postId: @event.Id);
                _logger.LogCommentDeletedFromEvent(@event.Id);
                return Result.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogCommentDeletionFromEventFailed(ex, @event.Id);
                return Result.Fail(CommentErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }

        public async Task<Result> HandleUserDeletedEventAsync(UserDeletedEvent @event)
        {
            try
            {
                await _repo.DeleteCommentsAsync(userId: @event.Id);
                _logger.LogCommentDeletedFromEvent(@event.Id);
                return Result.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogCommentDeletionFromEventFailed(ex, @event.Id);
                return Result.Fail(CommentErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }

        private async Task<List<CommentDto>> EnrichCommentsWithMetadataAsync(List<Comment> comments)
        {
            if (comments.Count == 0) return [];

            var commentIds = comments.Select(c => c.Id).ToList();

            var replyCountsTask = _repo.GetReplyCountsAsync(commentIds);
            var likeCountsTask = _likeDataClient.GetLikeCountsAsync(commentIds);

            await Task.WhenAll(replyCountsTask, likeCountsTask);

            var replyCounts = await replyCountsTask;
            var likeCountsResult = await likeCountsTask;
            var likeCounts = likeCountsResult.Success && likeCountsResult.Data?.Counts != null
                ? likeCountsResult.Data.Counts.ToDictionary(x => Guid.Parse(x.TargetId), x => x.Count)
                : [];

            return [.. comments.Select(comment =>
            {
                var dto = _mapper.Map<CommentDto>(comment);
                dto.ReplyCount = replyCounts.TryGetValue(comment.Id, out var replyCount) ? replyCount : 0;
                dto.LikeCount = likeCounts.TryGetValue(comment.Id, out var likeCount) ? likeCount : 0;
                return dto;
            })];
        }

        private async Task<CommentDto> EnrichCommentWithMetadataAsync(Comment comment)
        {
            var comments = new List<Comment> { comment };
            var dtos = await EnrichCommentsWithMetadataAsync(comments);
            return dtos.First();
        }
    }
}