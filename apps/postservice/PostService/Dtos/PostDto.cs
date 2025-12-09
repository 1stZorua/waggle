namespace Waggle.PostService.Dtos
{
    public class PostDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public required string Caption { get; set; }
        public required List<Guid> MediaIds { get; set; }
        public Dictionary<Guid, object>? MediaUrls { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
