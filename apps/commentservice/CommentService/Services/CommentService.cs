// Services/CommentService.cs
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Waggle.Common.Auth;
using Waggle.Common.Constants;
using Waggle.Common.Messaging;
using Waggle.Common.Pagination.Models;
using Waggle.Common.Results.Core;
using Waggle.Common.Validation;
using Waggle.CommentService.Constants;
using Waggle.CommentService.Data;
using Waggle.CommentService.Dtos;
using Waggle.CommentService.Logging;
using Waggle.CommentService.Models;
using Waggle.Contracts.Post.Extensions;
using Waggle.Contracts.Post.Interfaces;
using Waggle.Contracts.Comment.Events;

namespace Waggle.CommentService.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _repo;
        private readonly IMapper _mapper;
        private readonly IPostDataClient _postDataClient;
        private readonly IEventPublisher _eventPublisher;
        private readonly IServiceValidator _validator;
        private readonly ILogger<CommentService> _logger;

        public CommentService(
            ICommentRepository repo,
            IMapper mapper,
            IPostDataClient postDataClient,
            IEventPublisher eventPublisher,
            IServiceValidator validator,
            ILogger<CommentService> logger)
        {
            _repo = repo;
            _mapper = mapper;
            _postDataClient = postDataClient;
            _eventPublisher = eventPublisher;
            _validator = validator;
            _logger = logger;
        }

        public async Task<Result<PagedResult<CommentDto>>> GetCommentsAsync(PaginationRequest request)
        {
            try
            {
                var comments = await _repo.GetCommentsAsync(request: request);
                var dtos = _mapper.Map<List<CommentDto>>(comments.Items);

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
                var dto = _mapper.Map<CommentDto>(comment);
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
                var dtos = _mapper.Map<List<CommentDto>>(comments.Items);

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
                var comments = await _repo.GetCommentsAsync(parentCommentId: commentId, request: request);
                var dtos = _mapper.Map<List<CommentDto>>(comments.Items);

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
                var dtos = _mapper.Map<List<CommentDto>>(comments.Items);

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

        public async Task<Result<int>> GetCommentCountAsync(Guid postId)
        {
            try
            {
                var count = await _repo.GetCommentCountAsync(postId);
                _logger.LogCommentsRetrieved(count);
                return Result<int>.Ok(count);
            }
            catch (Exception ex)
            {
                _logger.LogCommentsRetrievalFailed(ex);
                return Result<int>.Fail(CommentErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }

        public async Task<Result<int>> GetReplyCountAsync(Guid commentId)
        {
            try
            {
                var count = await _repo.GetReplyCountAsync(commentId);
                _logger.LogCommentsRetrieved(count);
                return Result<int>.Ok(count);
            }
            catch (Exception ex)
            {
                _logger.LogCommentsRetrievalFailed(ex);
                return Result<int>.Fail(CommentErrors.Service.Failed, ErrorCodes.ServiceFailed);
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

                if (request.ParentCommentId.HasValue)
                {
                    var parentComment = await _repo.GetCommentByIdAsync(request.ParentCommentId.Value);
                    if (parentComment == null)
                    {
                        _logger.LogCommentParentNotFound(request.ParentCommentId.Value);
                        return Result<CommentDto>.Fail(CommentErrors.Comment.ParentCommentNotFound, ErrorCodes.NotFound);
                    }

                    if (parentComment.PostId != request.PostId)
                    {
                        _logger.LogCommentParentPostMismatch(request.ParentCommentId.Value, request.PostId);
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

                return Result<CommentDto>.Ok(_mapper.Map<CommentDto>(comment));
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

                return Result<CommentDto>.Ok(_mapper.Map<CommentDto>(comment));
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

            try
            {
                var comment = await _repo.GetCommentByIdAsync(id);
                if (comment == null)
                {
                    _logger.LogCommentDeleteNotFound(id);
                    return Result.Fail(CommentErrors.Comment.NotFound, ErrorCodes.NotFound);
                }

                bool isAdmin = currentUser.IsAdmin();
                bool isSelf = currentUser.IsSelf(comment.UserId);

                if (!isSelf && !isAdmin)
                {
                    _logger.LogUnauthorizedDeleteAttempt(currentUser.GetUserId(), id);
                    return Result.Fail(ErrorMessages.Authentication.PermissionRequired, ErrorCodes.Forbidden);
                }

                await _repo.DeleteCommentAsync(comment);

                var deletedEvent = _mapper.Map<CommentDeletedEvent>(comment);
                await _eventPublisher.PublishAsync(deletedEvent);

                _logger.LogCommentDeleted(id);
                return Result.Ok();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogCommentDeletionFailed(ex, id);
                return Result.Fail(CommentErrors.Comment.DeletionFailed, ErrorCodes.ServiceFailed);
            }
            catch (Exception ex)
            {
                _logger.LogCommentDeletionFailed(ex, id);
                return Result.Fail(CommentErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }
    }
}