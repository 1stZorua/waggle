using Waggle.Common.Results.Core;
using Waggle.MediaService.Dtos;

namespace Waggle.MediaService.Services
{
    public interface IMediaStorageService
    {
        Task<Result<UploadFileResponseDto>> UploadFileAsync(UploadFileRequestDto request);
        Task<Result<List<UploadFileResponseDto>>> UploadFilesAsync(IEnumerable<UploadFileRequestDto> request);
        Task<Result> DeleteFileAsync(DeleteFileRequestDto request);
        Task<Result> DeleteFilesAsync(DeleteFileRequestDto[] requests);
        Task<Result<UrlResponseDto>> GetFileUrlAsync(GetFileRequestDto request);
        Task<Result<Dictionary<string, UrlResponseDto>>> GetFileUrlsAsync(IEnumerable<GetFileRequestDto> requests);
    }
}
