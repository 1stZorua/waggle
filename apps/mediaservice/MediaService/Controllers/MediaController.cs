using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Waggle.Common.Controllers;
using Waggle.Common.Models;
using Waggle.Common.Pagination.Models;
using Waggle.Common.Results.Extensions;
using Waggle.MediaService.Dtos;
using Waggle.MediaService.Services;

namespace Waggle.MediaService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MediaController : BaseController
    {
        private readonly IMediaService _service;

        public MediaController(IMediaService service) => _service = service;

        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<MediaDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMedia([FromQuery] PaginationRequest request)
        {
            var result = await _service.GetAllMediaAsync(request);
            return result.ToActionResult();
        }

        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<MediaDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMediaById(Guid id)
        {
            var result = await _service.GetMediaByIdAsync(id);
            return result.ToActionResult();
        }

        [Authorize]
        [HttpPost("batch")]
        [ProducesResponseType(typeof(ApiResponse<List<MediaDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMediaBatch(MediaBatchRequest request)
        {
            var result = await _service.GetMediaByIdsAsync(request);
            return result.ToActionResult();
        }

        [Authorize]
        [HttpGet("{id}/url")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMediaUrl(Guid id)
        {
            var result = await _service.GetPresignedMediaUrlAsync(id);
            return result.ToActionResult();
        }

        [Authorize]
        [HttpPost("batch/urls")]
        [ProducesResponseType(typeof(ApiResponse<Dictionary<Guid, UrlResponseDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMediaUrls([FromBody] MediaBatchRequest request)
        {
            var result = await _service.GetPresignedMediaUrlsAsync(request);
            return result.ToActionResult();
        }

        [Authorize]
        [HttpPost]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(ApiResponse<MediaDto>), StatusCodes.Status201Created)]
        public async Task<IActionResult> UploadMedia([FromForm] MediaCreateDto request)
        {
            var result = await _service.UploadMediaAsync(request, CurrentUser);
            return result.ToCreatedResult($"/api/media/{result.Data?.Id}");
        }

        [Authorize]
        [HttpPost("batch/upload")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(ApiResponse<List<MediaDto>>), StatusCodes.Status201Created)]
        public async Task<IActionResult> UploadMediaBatch([FromForm] MediaBatchCreateDto request)
        {
            var result = await _service.UploadMediaBatchAsync(request, CurrentUser);
            return result.ToCreatedResult("/api/media");
        }

        [Authorize]
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteMedia(Guid id)
        {
            var result = await _service.DeleteMediaAsync(id, CurrentUser);
            return result.ToActionResult();
        }
    }
}