namespace Waggle.MediaService.Dtos
{
    public class MediaCreateDto
    {
        public required string BucketName { get; set; }
        public string? Prefix { get; set; }
        public required IFormFile File { get; set; }
    }
}
