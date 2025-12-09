using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Waggle.Common.Auth;
using Waggle.Common.Constants;
using Waggle.Common.Pagination.Models;
using Waggle.Common.Results.Core;
using Waggle.Common.Validation;
using Waggle.Contracts.Media.Extensions;
using Waggle.Contracts.Media.Grpc;
using Waggle.Contracts.Media.Interfaces;
using Waggle.PostService.Constants;
using Waggle.PostService.Data;
using Waggle.PostService.Dtos;
using Waggle.PostService.Logging;
using Waggle.PostService.Models;

namespace Waggle.PostService.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _repo;
        private readonly IMapper _mapper;
        private readonly IMediaDataClient _mediaDataClient;
        private readonly IServiceValidator _validator;
        private readonly ILogger<PostService> _logger;

        public PostService(IPostRepository repo, IMapper mapper, IMediaDataClient mediaDataClient, IServiceValidator validator, ILogger<PostService> logger)
        {
            _repo = repo;
            _mapper = mapper;
            _mediaDataClient = mediaDataClient;
            _validator = validator;
            _logger = logger;
        }

        public async Task<Result<PagedResult<PostDto>>> GetAllPostsAsync(PaginationRequest request)
        {
            try
            {
                var posts = await _repo.GetAllPostsAsync(request);

                var dtos = _mapper.Map<List<PostDto>>(posts.Items);

                var allMediaIds = dtos.SelectMany(p => p.MediaIds).Distinct().ToList();

                var mediaUrls = await _mediaDataClient.GetMediaUrlsAsync(
                    new GetMediaUrlsRequest { Ids = { allMediaIds.Select(id => id.ToString()) } }
                );
                if (!mediaUrls.Success || mediaUrls.Data == null)
                    return Result<PagedResult<PostDto>>.Fail(mediaUrls.Message, mediaUrls.ErrorCode);

                var allUrlsMapped = mediaUrls.Data.Urls.ToDictionary(
                    kvp => Guid.Parse(kvp.Key),
                    kvp => (object)new
                    {
                        kvp.Value.Url,
                        ExpiresAt = kvp.Value.ExpiresAt.ToDateTime()
                    }
                );

                foreach (var dto in dtos)
                {
                    dto.MediaUrls = dto.MediaIds
                        .Where(mid => allUrlsMapped.ContainsKey(mid))
                        .ToDictionary(mid => mid, mid => allUrlsMapped[mid]);
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

                var mediaUrls = await _mediaDataClient.GetMediaUrlsAsync(new ()
                    { Ids = { dto.MediaIds.Select(mid => mid.ToString()) } });
                if (!mediaUrls.Success || mediaUrls.Data == null)
                    return Result<PostDto>.Fail(mediaUrls.Message, mediaUrls.ErrorCode);

                dto.MediaUrls = mediaUrls.Data.Urls.ToDictionary(
                    kvp => Guid.Parse(kvp.Key),
                    kvp => (object)new
                    {
                        kvp.Value.Url,
                        ExpiresAt = kvp.Value.ExpiresAt.ToDateTime()
                    });

                return Result<PostDto>.Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogPostRetrievalFailed(ex, id);
                return Result<PostDto>.Fail(PostErrors.Service.Failed, ErrorCodes.ServiceFailed);
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
                var mediaUrlsResult = await _mediaDataClient.GetMediaUrlsAsync(
                    new GetMediaUrlsRequest { Ids = { request.MediaIds.Select(id => id.ToString()) } }
                );

                if (!mediaUrlsResult.Success || mediaUrlsResult.Data?.Urls.Count != request.MediaIds.Count)
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

            try
            {
                var post = await _repo.GetPostByIdAsync(id);
                if (post == null)
                {
                    _logger.LogPostDeleteNotFound(id);
                    return Result.Fail(PostErrors.Post.NotFound, ErrorCodes.NotFound);
                }

                bool isAdmin = currentUser.IsAdmin();
                bool isSelf = currentUser.IsSelf(post.UserId);

                if (!isSelf && !isAdmin)
                {
                    _logger.LogUnauthorizedDeleteAttempt(currentUser.GetUserId(), id);
                    return Result.Fail(ErrorMessages.Authentication.PermissionRequired, ErrorCodes.Forbidden);
                }

                await _repo.DeletePostAsync(post);

                _logger.LogPostDeleted(id);
                return Result.Ok();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogPostDeletionFailed(ex, id);
                return Result.Fail(PostErrors.Post.DeletionFailed, ErrorCodes.ServiceFailed);
            }
            catch (Exception ex)
            {
                _logger.LogPostDeletionFailed(ex, id);
                return Result.Fail(PostErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }
    }
}
