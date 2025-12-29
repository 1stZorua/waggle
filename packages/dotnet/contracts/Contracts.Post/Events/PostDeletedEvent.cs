namespace Waggle.Contracts.Post.Events
{
    public class PostDeletedEvent
    {
        // Metadata
        public string EventType { get; init; } = nameof(PostDeletedEvent);
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

        // Payload
        public Guid Id { get; set; }

        public IEnumerable<Guid> MediaIds { get; set; } = [];
    }
}
