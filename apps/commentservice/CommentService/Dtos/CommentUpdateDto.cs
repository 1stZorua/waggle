namespace Waggle.CommentService.Dtos
{
    public class CommentUpdateDto
    {
        public Guid PostId { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}
