namespace Waggle.Contracts.Comment.Events
{
    public class CommentUpdatedEvent
    {
        // Metadata
        public string EventType { get; init; } = nameof(CommentUpdatedEvent);
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

        // Payload
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid PostId { get; set; }
        public Guid? ParentId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; }
    }
}