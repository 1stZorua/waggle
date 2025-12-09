namespace Waggle.MediaService.Dtos
{
    public class MediaBatchRequest
    {
        public required IEnumerable<Guid> Ids { get; set; }
    }
}
