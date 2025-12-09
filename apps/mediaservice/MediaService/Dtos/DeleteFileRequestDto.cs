namespace Waggle.MediaService.Dtos
{
    public class DeleteFileRequestDto
    {
        public required string BucketName { get; set; }
        public required string ObjectName { get; set; }
    }
}
