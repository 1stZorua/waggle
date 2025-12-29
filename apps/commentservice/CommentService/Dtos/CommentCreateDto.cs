namespace Waggle.CommentService.Dtos
{
    public class CommentCreateDto
    {
        public Guid PostId { get; set; }
        public Guid? ParentCommentId { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}