namespace Waggle.MediaService.Dtos
{
    public class GetFileRequestDto
    {
        public required string BucketName { get; set; }
        public required string ObjectName { get; set; }
        public TimeSpan? Expiry { get; set; }
    }
}
