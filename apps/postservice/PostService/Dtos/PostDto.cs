namespace Waggle.PostService.Dtos
{
    public class PostDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public required string Caption { get; set; }
        public required Guid ThumbnailId { get; set; }
        public List<Guid>? MediaIds { get; set; } = [];
        public Dictionary<Guid, UrlResponseDto>? MediaUrls { get; set; }
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
