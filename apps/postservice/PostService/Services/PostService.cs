using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Waggle.Common.Auth;
using Waggle.Common.Constants;
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
        private readonly IServiceValidator _validator;
        private readonly ISagaCoordinator<DeletionSagaContext> _deletionSaga;
        private readonly ILogger<PostService> _logger;

        public PostService(
            IPostRepository repo, 
            IMapper mapper, 
            IMediaDataClient mediaDataClient, 
            ICommentDataClient commentDataClient,
            ILikeDataClient likeDataClient,
            IServiceValidator validator, 
            ISagaCoordinator<DeletionSagaContext> deletionSaga,
            ILogger<PostService> logger)
        {
            _repo = repo;
            _mapper = mapper;
            _mediaDataClient = mediaDataClient;
            _commentDataClient = commentDataClient;
            _likeDataClient = likeDataClient;
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

                var allMediaIds = dtos
                    .SelectMany(p => new[] { p.ThumbnailId }.Concat(p.MediaIds ?? []))
                    .Distinct()
                    .ToList();

                var mediaUrlsTask = _mediaDataClient.GetMediaUrlsAsync(
                    new GetMediaUrlsRequest { Ids = { allMediaIds.Select(id => id.ToString()) } }
                );

                var countTasks = dtos.Select(async dto =>
                {
                    var likeCountTask = _likeDataClient.GetLikeCountAsync(dto.Id);
                    var commentCountTask = _commentDataClient.GetCommentCountAsync(dto.Id);

                    await Task.WhenAll(likeCountTask, commentCountTask);

                    dto.LikeCount = likeCountTask.Result.Data?.Count ?? 0;
                    dto.CommentCount = commentCountTask.Result.Data?.Count ?? 0;
                }).ToList();

                await Task.WhenAll(countTasks.Append(mediaUrlsTask));

                var mediaUrls = await mediaUrlsTask;
                if (!mediaUrls.Success || mediaUrls.Data == null)
                    return Result<PagedResult<PostDto>>.Fail(mediaUrls.Message, mediaUrls.ErrorCode);

                var urlMap = mediaUrls.Data.Urls.ToDictionary(
                    kvp => Guid.Parse(kvp.Key),
                    kvp => new UrlResponseDto
                    {
                        Url = kvp.Value.Url,
                        ExpiresAt = kvp.Value.ExpiresAt.ToDateTime()
                    }
                );

                foreach (var dto in dtos)
                {
                    var ids = new[] { dto.ThumbnailId }.Concat(dto.MediaIds ?? []);
                    dto.MediaUrls = ids
                        .Select(id => (id, url: urlMap.GetValueOrDefault(id)))
                        .Where(x => x.url != null)
                        .ToDictionary(x => x.id, x => x.url!);
                }

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

                var ids = new[] { dto.ThumbnailId }.Concat(dto.MediaIds ?? []);

                var mediaUrlsTask = _mediaDataClient.GetMediaUrlsAsync(
                    new GetMediaUrlsRequest { Ids = { ids.Select(mid => mid.ToString()) } }
                );
                var likeCountTask = _likeDataClient.GetLikeCountAsync(id);
                var commentCountTask = _commentDataClient.GetCommentCountAsync(id);

                await Task.WhenAll(mediaUrlsTask, likeCountTask, commentCountTask);

                var mediaUrls = mediaUrlsTask.Result;
                if (!mediaUrls.Success || mediaUrls.Data == null)
                    return Result<PostDto>.Fail(mediaUrls.Message, mediaUrls.ErrorCode);

                var urlMap = mediaUrls.Data.Urls.ToDictionary(
                    kvp => Guid.Parse(kvp.Key),
                    kvp => new UrlResponseDto
                    {
                        Url = kvp.Value.Url,
                        ExpiresAt = kvp.Value.ExpiresAt.ToDateTime()
                    }
                );

                dto.MediaUrls = ids
                    .Where(id => urlMap.ContainsKey(id))
                    .ToDictionary(id => id, id => urlMap[id]);

                dto.LikeCount = likeCountTask.Result.Data?.Count ?? 0;
                dto.CommentCount = commentCountTask.Result.Data?.Count ?? 0;

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

                var allMediaIds = dtos
                    .SelectMany(p => new[] { p.ThumbnailId }.Concat(p.MediaIds ?? []))
                    .Distinct()
                    .ToList();

                var mediaUrlsTask = _mediaDataClient.GetMediaUrlsAsync(
                    new GetMediaUrlsRequest { Ids = { allMediaIds.Select(id => id.ToString()) } }
                );

                var countTasks = dtos.Select(async dto =>
                {
                    var likeCountTask = _likeDataClient.GetLikeCountAsync(dto.Id);
                    var commentCountTask = _commentDataClient.GetCommentCountAsync(dto.Id);

                    await Task.WhenAll(likeCountTask, commentCountTask);

                    dto.LikeCount = likeCountTask.Result.Data?.Count ?? 0;
                    dto.CommentCount = commentCountTask.Result.Data?.Count ?? 0;
                }).ToList();

                await Task.WhenAll(countTasks.Append(mediaUrlsTask));

                var mediaUrls = await mediaUrlsTask;
                if (!mediaUrls.Success || mediaUrls.Data == null)
                    return Result<PagedResult<PostDto>>.Fail(mediaUrls.Message, mediaUrls.ErrorCode);

                var urlMap = mediaUrls.Data.Urls.ToDictionary(
                    kvp => Guid.Parse(kvp.Key),
                    kvp => new UrlResponseDto
                    {
                        Url = kvp.Value.Url,
                        ExpiresAt = kvp.Value.ExpiresAt.ToDateTime()
                    }
                );

                foreach (var dto in dtos)
                {
                    var ids = new[] { dto.ThumbnailId }.Concat(dto.MediaIds ?? []);
                    dto.MediaUrls = ids
                        .Select(id => (id, url: urlMap.GetValueOrDefault(id)))
                        .Where(x => x.url != null)
                        .ToDictionary(x => x.id, x => x.url!);
                }

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
    }
}
