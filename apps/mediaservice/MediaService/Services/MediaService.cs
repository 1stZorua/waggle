using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Waggle.Common.Auth;
using Waggle.Common.Constants;
using Waggle.Common.Pagination.Models;
using Waggle.Common.Results.Core;
using Waggle.Common.Validation;
using Waggle.Contracts.Auth.Events;
using Waggle.Contracts.Post.Events;
using Waggle.Mediaervice.Logging;
using Waggle.MediaService.Constants;
using Waggle.MediaService.Data;
using Waggle.MediaService.Dtos;
using Waggle.MediaService.Models;

namespace Waggle.MediaService.Services
{
    public class MediaService : IMediaService
    {
        private readonly IMediaRepository _repo;
        private readonly IMediaStorageService _storage;
        private readonly IMapper _mapper;
        private readonly IServiceValidator _validator;
        private readonly ILogger<MediaService> _logger;

        public MediaService(IMediaRepository repo, IMediaStorageService storage, IMapper mapper, IServiceValidator validator, ILogger<MediaService> logger)
        {
            _repo = repo;
            _storage = storage;
            _mapper = mapper;
            _validator = validator;
            _logger = logger;
        }

        public async Task<Result<PagedResult<MediaDto>>> GetAllMediaAsync(PaginationRequest request)
        {
            try
            {
                var media = await _repo.GetAllMediaAsync(request);

                var pagedResult = new PagedResult<MediaDto>
                {
                    Items = _mapper.Map<List<MediaDto>>(media.Items),
                    PageInfo = media.PageInfo
                };

                _logger.LogMediaRetrieved(pagedResult.Items.Count);
                return Result<PagedResult<MediaDto>>.Ok(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogAllMediaRetrievalFailed(ex);
                return Result<PagedResult<MediaDto>>.Fail(MediaErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }

        public async Task<Result<MediaDto>> GetMediaByIdAsync(Guid id)
        {
            try
            {
                var media = await _repo.GetMediaByIdAsync(id);
                if (media == null)
                {
                    _logger.LogMediaNotFound(id);
                    return Result<MediaDto>.Fail(MediaErrors.Media.NotFound, ErrorCodes.NotFound);
                }

                _logger.LogMediaRetrieved(id);

                var dto = _mapper.Map<MediaDto>(media);
                return Result<MediaDto>.Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogMediaRetrievalFailed(ex, id);
                return Result<MediaDto>.Fail(MediaErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }

        public async Task<Result<List<MediaDto>>> GetMediaByIdsAsync(MediaBatchRequest request)
        {
            try
            {
                var media = await _repo.GetMediaByIdsAsync(request);

                var dtos = _mapper.Map<List<MediaDto>>(media);
                return Result<List<MediaDto>>.Ok(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogMediaBatchRetrievalFailed(ex);
                return Result<List<MediaDto>>.Fail(MediaErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }

        public async Task<Result<UrlResponseDto>> GetPresignedMediaUrlAsync(Guid id)
        {
            try
            {
                var media = await _repo.GetMediaByIdAsync(id);
                if (media == null)
                {
                    _logger.LogMediaNotFound(id);
                    return Result<UrlResponseDto>.Fail(MediaErrors.Media.NotFound, ErrorCodes.NotFound);
                }

                return await _storage.GetFileUrlAsync(new () { BucketName = media.BucketName, ObjectName = media.ObjectName });
            }
            catch (Exception ex)
            {
                _logger.LogMediaRetrievalFailed(ex, id);
                return Result<UrlResponseDto>.Fail(MediaErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }
        public async Task<Result<Dictionary<Guid, UrlResponseDto>>> GetPresignedMediaUrlsAsync(MediaBatchRequest request)
        {
            try
            {
                var mediaList = await _repo.GetMediaByIdsAsync(request);

                var storageRequests = mediaList.Select(m => new GetFileRequestDto
                {
                    BucketName = m.BucketName,
                    ObjectName = m.ObjectName
                });

                var urlsResult = await _storage.GetFileUrlsAsync(storageRequests);
                if (!urlsResult.Success || urlsResult.Data == null)
                    return Result<Dictionary<Guid, UrlResponseDto>>.Fail(urlsResult.Message, urlsResult.ErrorCode);

                var resultMap = mediaList
                    .Where(m => urlsResult.Data.ContainsKey(m.ObjectName))
                    .ToDictionary(m => m.Id, m => urlsResult.Data[m.ObjectName]);

                return Result<Dictionary<Guid, UrlResponseDto>>.Ok(resultMap);

            } 
            catch (Exception)
            {
                return Result<Dictionary<Guid, UrlResponseDto>>.Fail(MediaErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }

        public async Task<Result<MediaDto>> UploadMediaAsync(MediaCreateDto request, UserInfoDto currentUser)
        {
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.Success)
                return Result<MediaDto>.ValidationFail(validationResult.ValidationErrors!);

            if (!currentUser.TryEnsure<MediaDto>(out var failedResult))
                return failedResult;

            var userId = currentUser.GetUserId();

            try
            {
                var uploadResult = await _storage.UploadFileAsync(new()
                {
                    BucketName = request.BucketName,
                    Prefix = request.Prefix,
                    File = request.File
                });

                if (!uploadResult.Success || uploadResult.Data == null)
                    return Result<MediaDto>.Fail(uploadResult.Message, uploadResult.ErrorCode);

                var media = _mapper.Map<Media>(request);

                media.UploaderId = userId;
                media.ObjectName = uploadResult.Data.ObjectName;
                media.FileName = request.File.FileName;
                media.ContentType = request.File.ContentType;
                media.FileSize = request.File.Length;
                media.CreatedAt = DateTime.UtcNow;
                media.UpdatedAt = DateTime.UtcNow;

                await _repo.AddMediaAsync(media);

                _logger.LogMediaUploaded(media.Id, userId);

                var result = _mapper.Map<MediaDto>(media);
                return Result<MediaDto>.Ok(result);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogDatabaseUpdateFailed(ex, userId);
                return Result<MediaDto>.Fail(MediaErrors.Media.CreationFailed, ErrorCodes.ServiceFailed);
            }
            catch (Exception ex)
            {
                _logger.LogMediaCreationFailed(ex, userId);
                return Result<MediaDto>.Fail(MediaErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }

        public async Task<Result<List<MediaDto>>> UploadMediaBatchAsync(MediaBatchCreateDto request, UserInfoDto currentUser)
        {
            if (!currentUser.TryEnsure<List<MediaDto>>(out var failedResult))
                return failedResult;

            var userId = currentUser.GetUserId();

            try
            {
                var uploadRequests = request.Files.Select(file => new UploadFileRequestDto
                {
                    BucketName = request.BucketName,
                    Prefix = request.Prefix,
                    File = file
                }).ToList();

                var uploadResult = await _storage.UploadFilesAsync(uploadRequests);
                if (!uploadResult.Success || uploadResult.Data == null)
                    return Result<List<MediaDto>>.Fail(uploadResult.Message, uploadResult.ErrorCode);

                var mediaList = uploadResult.Data
                    .Zip(request.Files, (uploaded, file) => new Media
                    {
                        UploaderId = userId,
                        BucketName = request.BucketName,
                        ObjectName = uploaded.ObjectName,
                        FileName = file.FileName,
                        ContentType = file.ContentType,
                        FileSize = file.Length,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    })
                    .ToList();

                await _repo.AddMediaBatchAsync(mediaList);

                var mediaDtos = mediaList.Select(m => _mapper.Map<MediaDto>(m)).ToList();

                return Result<List<MediaDto>>.Ok(mediaDtos);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogDatabaseUpdateFailed(ex, userId);
                return Result<List<MediaDto>>.Fail(MediaErrors.Media.CreationFailed, ErrorCodes.ServiceFailed);
            }
            catch (Exception ex)
            {
                _logger.LogMediaCreationFailed(ex, userId);
                return Result<List<MediaDto>>.Fail(MediaErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }

        public async Task<Result> DeleteMediaAsync(Guid id, UserInfoDto currentUser)
        {
            if (!currentUser.TryEnsure(out var failedResult))
                return failedResult;

            var userId = currentUser.GetUserId();

            try
            {
                var media = await _repo.GetMediaByIdAsync(id);
                if (media == null)
                {
                    _logger.LogMediaDeleteNotFound(id);
                    return Result.Fail(MediaErrors.Media.NotFound, ErrorCodes.NotFound);
                }

                bool isAdmin = currentUser.IsAdmin();
                bool isSelf = currentUser.IsSelf(media.UploaderId);

                if (!isSelf && !isAdmin)
                {
                    _logger.LogUnauthorizedDeleteAttempt(userId, id);
                    return Result.Fail(ErrorMessages.Authentication.PermissionRequired, ErrorCodes.Forbidden);
                }

                var storageResult = await _storage.DeleteFileAsync(new()
                {
                    BucketName = media.BucketName,
                    ObjectName = media.ObjectName
                });

                if (!storageResult.Success)
                {
                    return Result.Fail(storageResult.Message, storageResult.ErrorCode);
                }

                await _repo.DeleteMediaAsync(media);

                _logger.LogMediaDeleted(id);
                return Result.Ok();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogMediaDeletionFailed(ex, id);
                return Result.Fail(MediaErrors.Media.DeletionFailed, ErrorCodes.ServiceFailed);
            }
            catch (Exception ex)
            {
                _logger.LogMediaDeletionFailed(ex, id);
                return Result.Fail(MediaErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }

        public async Task<Result> HandlePostDeletedEventAsync(PostDeletedEvent @event)
        {
            try
            {
                var media = await _repo.GetMediaByIdsAsync(new() { Ids = @event.MediaIds });
                if (media.Count == 0)
                    return Result.Ok();

                var storageResult = await _storage.DeleteFilesAsync(
                    [.. media.Select(m => new DeleteFileRequestDto
                    {
                        BucketName = m.BucketName,
                        ObjectName = m.ObjectName
                    })]
                );

                if (!storageResult.Success)
                    return storageResult;

                await _repo.DeleteAllMediaByIdsAsync(new() { Ids = @event.MediaIds });
                return Result.Ok();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogMediaDeletionFromEventFailed(ex, @event.Id);
                return Result.Fail(MediaErrors.Media.DeletionFailed, ErrorCodes.ServiceFailed);
            }
            catch (Exception ex)
            {
                _logger.LogMediaDeletionFromEventFailed(ex, @event.Id);
                return Result.Fail(MediaErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }

        public async Task<Result> HandleUserDeletedEventAsync(UserDeletedEvent @event)
        {
            try
            {
                var media = await _repo.GetAllMediaByUploaderIdAsync(@event.Id);
                if (media.Count == 0)
                    return Result.Ok();

                var storageResult = await _storage.DeleteFilesAsync(
                    [.. media.Select(m => new DeleteFileRequestDto
                    {
                        BucketName = m.BucketName,
                        ObjectName = m.ObjectName
                    })]
                );

                if (!storageResult.Success)
                    return storageResult;

                await _repo.DeleteAllMediaByUploaderIdAsync(@event.Id);
                return Result.Ok();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogMediaDeletionFromEventFailed(ex, @event.Id);
                return Result.Fail(MediaErrors.Media.DeletionFailed, ErrorCodes.ServiceFailed);
            }
            catch (Exception ex)
            {
                _logger.LogMediaDeletionFromEventFailed(ex, @event.Id);
                return Result.Fail(MediaErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }
    }
}
