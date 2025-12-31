namespace Waggle.Contracts.Post.Events
{
    public class PostCreatedEvent
    {
        // Metadata
        public string EventType { get; init; } = nameof(PostCreatedEvent);
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

        // Payload
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Caption { get; set; } = string.Empty;
        public Guid ThumbnailId { get; set; }
        public IEnumerable<Guid> MediaIds { get; set; } = [];
        public DateTime PostCreatedAt { get; set; }
    }
}