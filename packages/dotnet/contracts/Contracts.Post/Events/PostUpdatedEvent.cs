namespace Waggle.Contracts.Post.Events
{
    public class PostUpdatedEvent
    {
        // Metadata
        public string EventType { get; init; } = nameof(PostUpdatedEvent);
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

        // Payload
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Caption { get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; }
    }
}