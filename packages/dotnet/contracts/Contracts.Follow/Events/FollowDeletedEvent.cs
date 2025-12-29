namespace Waggle.Contracts.Follow.Events
{
    public class FollowDeletedEvent
    {
        // Metadata
        public string EventType { get; init; } = nameof(FollowDeletedEvent);
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

        // Payload
        public Guid Id { get; set; }
        public Guid FollowerId { get; set; }
        public Guid FollowingId { get; set; }
    }
}