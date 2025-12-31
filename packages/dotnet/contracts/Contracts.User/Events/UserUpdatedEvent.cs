namespace Waggle.Contracts.User.Events
{
    public class UserUpdatedEvent
    {
        // Metadata
        public string EventType { get; init; } = nameof(UserUpdatedEvent);
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

        // Payload
        public Guid Id { get; set; }
        public Guid AvatarId { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
