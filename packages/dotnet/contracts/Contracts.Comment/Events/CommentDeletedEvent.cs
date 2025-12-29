namespace Waggle.Contracts.Comment.Events
{
    public class CommentDeletedEvent
    {
        // Metadata
        public string EventType { get; init; } = nameof(CommentDeletedEvent);
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

        // Payload
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid PostId { get; set; }
        public Guid? ParentCommentId { get; set; }
    }
}