using Minio;
using Minio.DataModel.Args;
using Waggle.Common.Constants;
using Waggle.Common.Results.Core;
using Waggle.MediaService.Constants;
using Waggle.MediaService.Dtos;

namespace Waggle.MediaService.Services
{
    public class MinioStorageService : IMediaStorageService
    {
        private readonly IMinioClient _minioClient;
        private readonly string? _externalHost;
        private static TimeSpan DefaultExpiry => TimeSpan.FromMinutes(15);

        public MinioStorageService(IMinioClient minioClient, IConfiguration config)
        {
            _minioClient = minioClient;
            _externalHost = config["Minio:ExternalHost"];
        }

        public async Task<Result<UploadFileResponseDto>> UploadFileAsync(UploadFileRequestDto request)
        {
            try
            {
                var objectName = request.Prefix != null
                    ? $"{request.Prefix}/{Guid.NewGuid()}_{request.File.FileName}"
                    : $"{Guid.NewGuid()}_{request.File.FileName}";

                using var stream = request.File.OpenReadStream();
                await _minioClient.PutObjectAsync(new PutObjectArgs()
                    .WithBucket(request.BucketName)
                    .WithObject(objectName)
                    .WithStreamData(stream)
                    .WithObjectSize(stream.Length)
                    .WithContentType(request.File.ContentType)
                );

                var fileResult = await GetFileUrlAsync(new()
                {
                    BucketName = request.BucketName,
                    ObjectName = objectName
                });

                if (!fileResult.Success || fileResult.Data == null)
                {
                    return Result<UploadFileResponseDto>.Fail(fileResult.Message, fileResult.ErrorCode);
                }

                return Result<UploadFileResponseDto>.Ok(new()
                {
                    ObjectName = objectName,
                    Url = fileResult.Data.Url
                });
            }
            catch (Exception) 
            {
                return Result<UploadFileResponseDto>.Fail(MediaErrors.Media.UploadFailed, ErrorCodes.ServiceFailed);
            }
        }

        public async Task<Result<List<UploadFileResponseDto>>> UploadFilesAsync(IEnumerable<UploadFileRequestDto> request)
        {
            var tasks = request.Select(r => UploadFileAsync(r));
            var results = await Task.WhenAll(tasks);

            var failed = results.FirstOrDefault(r => !r.Success || r.Data == null);
            if (failed != null)
                return Result<List<UploadFileResponseDto>>.Fail(failed.Message, failed.ErrorCode);

            return Result<List<UploadFileResponseDto>>.Ok([.. results.Select(r => r.Data!)]);
        }

        public async Task<Result> DeleteFileAsync(DeleteFileRequestDto request)
        {
            try
            {
                await _minioClient.RemoveObjectAsync(new RemoveObjectArgs()
                    .WithBucket(request.BucketName)
                    .WithObject(request.ObjectName)
                );

                return Result.Ok();
            }
            catch (Exception)
            {
                return Result.Fail(MediaErrors.Media.StorageDeletionFailed, ErrorCodes.ServiceFailed);
            }
        }

        public async Task<Result> DeleteFilesAsync(DeleteFileRequestDto[] requests)
        {
            try
            {
                if (requests.Length == 0)
                    return Result.Ok();

                var groupedByBucket = requests.GroupBy(r => r.BucketName);

                foreach (var bucketGroup in groupedByBucket)
                {
                    var objectNames = bucketGroup
                        .Select(r => r.ObjectName)
                        .Distinct()
                        .ToList();

                    if (objectNames.Count == 0)
                        continue;

                    var removeArgs = new RemoveObjectsArgs()
                        .WithBucket(bucketGroup.Key)
                        .WithObjects(objectNames);

                    await _minioClient.RemoveObjectsAsync(removeArgs);
                }

                return Result.Ok();
            }
            catch
            {
                return Result.Fail(
                    MediaErrors.Media.StorageDeletionFailed,
                    ErrorCodes.ServiceFailed
                );
            }
        }

        public async Task<Result<UrlResponseDto>> GetFileUrlAsync(GetFileRequestDto request)
        {
            try
            {
                var expirySeconds = (int)(request.Expiry ?? DefaultExpiry).TotalSeconds;

                //var presignedUrl = await _minioClient.PresignedGetObjectAsync(
                //    new PresignedGetObjectArgs()
                //        .WithBucket(request.BucketName)
                //        .WithObject(request.ObjectName)
                //        .WithExpiry(expirySeconds)
                //);

                //if (!string.IsNullOrEmpty(_externalHost))
                //{
                //    var externalUri = new Uri(_externalHost);
                //    var uri = new Uri(presignedUrl);
                //    var builder = new UriBuilder(uri)
                //    {
                //        Scheme = externalUri.Scheme,
                //        Host = externalUri.Host,
                //        Port = externalUri.Port
                //    };
                //    presignedUrl = builder.ToString();
                //}

                var baseUrl = _externalHost?.TrimEnd('/');
                var url = $"{baseUrl}/{request.BucketName}/{request.ObjectName}";

                var dto = new UrlResponseDto
                {
                    Url = url,
                    ExpiresAt = DateTime.UtcNow.AddSeconds(expirySeconds)
                };

                return Result<UrlResponseDto>.Ok(dto);
            } 
            catch (Exception)
            {
                return Result<UrlResponseDto>.Fail(MediaErrors.Media.GenerateUrlFailed, ErrorCodes.ServiceFailed);
            }
        }

        public async Task<Result<Dictionary<string, UrlResponseDto>>> GetFileUrlsAsync(IEnumerable<GetFileRequestDto> requests)
        {
            try
            {
                var tasks = requests.Select(async r =>
                {
                    var urlResult = await GetFileUrlAsync(r);
                    return new { r.ObjectName, urlResult };
                });

                var results = await Task.WhenAll(tasks);

                var dictionary = results
                    .Where(r => r.urlResult.Success && r.urlResult.Data != null)
                    .ToDictionary(r => r.ObjectName, r => r.urlResult.Data!);

                return Result<Dictionary<string, UrlResponseDto>>.Ok(dictionary);
            }
            catch (Exception)
            {
                return Result<Dictionary<string, UrlResponseDto>>.Fail(MediaErrors.Service.Failed, ErrorCodes.ServiceFailed);
            }
        }
    }
}
