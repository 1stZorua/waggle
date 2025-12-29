namespace Waggle.PostService.Dtos
{
    public class PostCreateDto
    {
        public required string Caption { get; set; }
        public required Guid ThumbnailId { get; set; }
        public List<Guid>? MediaIds { get; set; } = [];
    }
}
