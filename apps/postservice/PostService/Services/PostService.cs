using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Waggle.Common.Auth;
using Waggle.Common.Constants;
using Waggle.Common.Messaging;
using Waggle.Common.Pagination.Models;
using Waggle.Common.Results.Core;
using Waggle.Common.Saga;
using Waggle.Common.Validation;
using Waggle.Contracts.Auth.Events;
using Waggle.Contracts.Comment.Extensions;
using Waggle.Contracts.Comment.Grpc;
using Waggle.Contracts.Comment.Interfaces;
using Waggle.Contracts.Like.Extensions;
using Waggle.Contracts.Like.Grpc;
using Waggle.Contracts.Like.Interfaces;
using Waggle.Contracts.Media.Grpc;
using Waggle.Contracts.Media.Interfaces;
using Waggle.Contracts.Post.Events;
using Waggle.PostService.Constants;
using Waggle.PostService.Data;
using Waggle.PostService.Dtos;
using Waggle.PostService.Logging;
using Waggle.PostService.Models;
using Waggle.PostService.Saga.Context;

namespace Waggle.PostService.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _repo;
        private readonly IMapper _mapper;
        private readonly IMediaDataClient _mediaDataClient;
        private readonly ICommentDataClient _commentDataClient;
        private readonly ILikeDataClient _likeDataClient;
        private readonly IEventPublisher _eventPublisher;
        private readonly IServiceValidator _validator;
        private readonly ISagaCoordinator<DeletionSagaContext> _deletionSaga;
        private readonly ILogger<PostService> _logger;

        public PostService(
            IPostRepository repo,
            IMapper mapper,
            IMediaDataClient mediaDataClient,
            ICommentDataClient commentDataClient,
            ILikeDataClient likeDataClient,
            IEventPublisher eventPublisher,
            IServiceValidator validator,
            ISagaCoordinator<DeletionSagaContext> deletionSaga,
            ILogger<PostService> logger)
        {
            _repo = repo;
            _mapper = mapper;
            _mediaDataClient = mediaDataClient;
            _commentDataClient = commentDataClient;
            _likeDataClient = likeDataClient;
            _eventPublisher = eventPublisher;
            _validator = validator;
            _deletionSaga = deletionSaga;
            _logger = logger;
        }

        public async Task<Result<PagedResult<PostDto>>> GetPostsAsync(PaginationRequest request)
        {
            try
            {
                var posts = await _repo.GetPostsAsync(request);
                var dtos = _mapper.Map<List<PostDto>>(posts.Items);

                await EnrichPostsWithMetadataAsync(dtos);

                var pagedResult = new PagedResult<PostDto>
                {
                    Items = dtos,
                    PageInfo = posts.PageInfo
                };

                _logger.LogPostsRetrieved(pagedResult.Items.Count);
                return Result<PagedResult<PostDto>>.Ok(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogPostsRetrievalFailed(ex);
                return Result<PagedResult<PostDto>>.Fail(PostErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }

        public async Task<Result<PostDto>> GetPostByIdAsync(Guid id)
        {
            try
            {
                var post = await _repo.GetPostByIdAsync(id);
                if (post == null)
                {
                    _logger.LogPostNotFound(id);
                    return Result<PostDto>.Fail(PostErrors.Post.NotFound, ErrorCodes.NotFound);
                }

                _logger.LogPostRetrieved(id);

                var dto = _mapper.Map<PostDto>(post);
                await EnrichPostsWithMetadataAsync(new[] { dto }.ToList());

                return Result<PostDto>.Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogPostRetrievalFailed(ex, id);
                return Result<PostDto>.Fail(PostErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }

        public async Task<Result<PagedResult<PostDto>>> GetPostsByUserIdAsync(Guid userId, PaginationRequest request)
        {
            try
            {
                var posts = await _repo.GetPostsByUserIdAsync(userId, request);
                var dtos = _mapper.Map<List<PostDto>>(posts.Items);

                await EnrichPostsWithMetadataAsync(dtos);

                var pagedResult = new PagedResult<PostDto>
                {
                    Items = dtos,
                    PageInfo = posts.PageInfo
                };

                _logger.LogPostsRetrieved(pagedResult.Items.Count);
                return Result<PagedResult<PostDto>>.Ok(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogPostsRetrievalFailed(ex);
                return Result<PagedResult<PostDto>>.Fail(PostErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }

        public async Task<Result<Dictionary<Guid, int>>> GetPostCountsAsync(IEnumerable<Guid> userIds)
        {
            try
            {
                var counts = await _repo.GetPostCountsAsync(userIds);
                return Result<Dictionary<Guid, int>>.Ok(counts);
            }
            catch (Exception)
            {
                return Result<Dictionary<Guid, int>>.Fail(PostErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }

        public async Task<Result<PostDto>> CreatePostAsync(PostCreateDto request, UserInfoDto currentUser)
        {
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.Success)
                return Result<PostDto>.ValidationFail(validationResult.ValidationErrors!);

            if (!currentUser.TryEnsure<PostDto>(out var failedResult))
                return failedResult;

            var userId = currentUser.GetUserId();

            try
            {
                var mediaIds = new[] { request.ThumbnailId.ToString() }
                    .Concat(request.MediaIds?.Select(id => id.ToString()) ?? [])
                    .ToList();

                var mediaUrlsResult = await _mediaDataClient.GetMediaUrlsAsync(
                    new GetMediaUrlsRequest { Ids = { mediaIds } }
                );

                if (!mediaUrlsResult.Success || mediaUrlsResult.Data?.Urls.Count != mediaIds.Count)
                {
                    _logger.LogPostCreationFailedMedia(userId);
                    return Result<PostDto>.Fail(PostErrors.Post.MediaDoesNotExist, ErrorCodes.InvalidInput);
                }

                var post = _mapper.Map<Post>(request);

                post.UserId = userId;
                post.CreatedAt = DateTime.UtcNow;
                post.UpdatedAt = DateTime.UtcNow;

                await _repo.AddPostAsync(post);

                var createdEvent = _mapper.Map<PostCreatedEvent>(post);
                await _eventPublisher.PublishAsync(createdEvent);

                _logger.LogPostCreated(post.Id, post.UserId);

                var result = _mapper.Map<PostDto>(post);
                return Result<PostDto>.Ok(result);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogDatabaseUpdateFailed(ex, userId);
                return Result<PostDto>.Fail(PostErrors.Post.CreationFailed, ErrorCodes.ServiceFailed);
            }
            catch (Exception ex)
            {
                _logger.LogPostCreationFailed(ex, userId);
                return Result<PostDto>.Fail(PostErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }

        public async Task<Result<PostDto>> UpdatePostAsync(Guid id, PostUpdateDto request, UserInfoDto currentUser)
        {
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.Success)
                return Result<PostDto>.ValidationFail(validationResult.ValidationErrors!);

            if (!currentUser.TryEnsure<PostDto>(out var failedResult))
                return failedResult;

            var userId = currentUser.GetUserId();

            try
            {
                var post = await _repo.GetPostByIdAsync(id);
                if (post == null)
                {
                    _logger.LogPostNotFound(id);
                    return Result<PostDto>.Fail(PostErrors.Post.NotFound, ErrorCodes.NotFound);
                }

                if (post.UserId != userId)
                {
                    _logger.LogUnauthorizedPostAccess(id, userId);
                    return Result<PostDto>.Fail(ErrorMessages.Authentication.PermissionRequired, ErrorCodes.Forbidden);
                }

                post.Caption = request.Caption;
                post.UpdatedAt = DateTime.UtcNow;

                await _repo.UpdatePostAsync(post);

                var updatedEvent = _mapper.Map<PostUpdatedEvent>(post);
                await _eventPublisher.PublishAsync(updatedEvent);

                var dto = _mapper.Map<PostDto>(post);
                await EnrichPostsWithMetadataAsync([dto]);

                _logger.LogPostUpdated(post.Id, post.UserId);
                return Result<PostDto>.Ok(dto);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogDatabaseUpdateFailed(ex, userId);
                return Result<PostDto>.Fail(PostErrors.Post.UpdateFailed, ErrorCodes.ServiceFailed);
            }
            catch (Exception ex)
            {
                _logger.LogPostUpdateFailed(ex, id, userId);
                return Result<PostDto>.Fail(PostErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }

        public async Task<Result> DeletePostAsync(Guid id, UserInfoDto currentUser)
        {
            if (!currentUser.TryEnsure(out var failedResult))
                return failedResult;

            var sagaContext = new DeletionSagaContext()
            {
                Id = id,
                CurrentUser = currentUser
            };

            return await _deletionSaga.ExecuteAsync(sagaContext);
        }

        public async Task<Result> HandleUserDeletedEventAsync(UserDeletedEvent @event)
        {
            try
            {
                await _repo.DeletePostsByUserIdAsync(@event.Id);
                return Result.Ok();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogPostDeletionFromEventFailed(ex, @event.Id);
                return Result.Fail(PostErrors.Post.DeletionFailed, ErrorCodes.ServiceFailed);
            }
            catch (Exception ex)
            {
                _logger.LogPostDeletionFromEventFailed(ex, @event.Id);
                return Result.Fail(PostErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }

        private async Task EnrichPostsWithMetadataAsync(List<PostDto> dtos)
        {
            if (dtos.Count == 0) return;

            var allMediaIds = dtos
                .SelectMany(p => new[] { p.ThumbnailId }.Concat(p.MediaIds ?? []))
                .Distinct()
                .ToList();

            var postIds = dtos.Select(d => d.Id).ToList();

            var mediaUrlsTask = _mediaDataClient.GetMediaUrlsAsync(
                new GetMediaUrlsRequest { Ids = { allMediaIds.Select(id => id.ToString()) } }
            );
            var likeCountsTask = _likeDataClient.GetLikeCountsAsync(postIds);
            var commentCountsTask = _commentDataClient.GetCommentCountsAsync(postIds);

            await Task.WhenAll(mediaUrlsTask, likeCountsTask, commentCountsTask);

            var mediaUrls = await mediaUrlsTask;
            var urlMap = mediaUrls.Success && mediaUrls.Data != null
                ? mediaUrls.Data.Urls.ToDictionary(
                    kvp => Guid.Parse(kvp.Key),
                    kvp => new UrlResponseDto
                    {
                        Url = kvp.Value.Url,
                        ExpiresAt = kvp.Value.ExpiresAt.ToDateTime()
                    }
                )
                : [];

            var likeCounts = ConvertLikeCountsToDictionary(await likeCountsTask);
            var commentCounts = ConvertCommentCountsToDictionary(await commentCountsTask);

            foreach (var dto in dtos)
            {
                var ids = new[] { dto.ThumbnailId }.Concat(dto.MediaIds ?? []);
                dto.MediaUrls = ids
                    .Select(id => (id, url: urlMap.GetValueOrDefault(id)))
                    .Where(x => x.url != null)
                    .ToDictionary(x => x.id, x => x.url!);

                dto.LikeCount = likeCounts.TryGetValue(dto.Id, out var likeCount) ? likeCount : 0;
                dto.CommentCount = commentCounts.TryGetValue(dto.Id, out var commentCount) ? commentCount : 0;
            }
        }

        private static Dictionary<Guid, int> ConvertLikeCountsToDictionary(Result<GetLikeCountsResponse> result)
        {
            if (!result.Success || result.Data?.Counts == null)
                return [];

            return result.Data.Counts.ToDictionary(
                x => Guid.Parse(x.TargetId),
                x => x.Count
            );
        }

        private static Dictionary<Guid, int> ConvertCommentCountsToDictionary(Result<GetCommentCountsResponse> result)
        {
            if (!result.Success || result.Data?.Counts == null)
                return [];

            return result.Data.Counts.ToDictionary(
                x => Guid.Parse(x.PostId),
                x => x.Count
            );
        }
    }
}