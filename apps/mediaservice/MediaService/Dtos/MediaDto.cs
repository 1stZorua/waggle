namespace Waggle.MediaService.Dtos
{
    public class MediaDto
    {
        public Guid Id { get; set; }
        public Guid UploaderId { get; set; }
        public required string BucketName { get; set; }
        public required string ObjectName { get; set; }
        public required string FileName { get; set; }
        public required string ContentType { get; set; }
        public long FileSize { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
