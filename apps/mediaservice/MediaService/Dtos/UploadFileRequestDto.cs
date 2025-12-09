namespace Waggle.MediaService.Dtos
{
    public class UploadFileRequestDto
    {
        public required string BucketName { get; set; }
        public string? Prefix { get; set; }
        public required IFormFile File { get; set; }
    }
}
