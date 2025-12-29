namespace Waggle.MediaService.Dtos
{
    public class MediaBatchCreateDto
    {
        public required string BucketName { get; set; }
        public string? Prefix { get; set; }
        public required List<IFormFile> Files { get; set; }
    }
}
