namespace Waggle.Contracts.Auth.Events
{
    public class UserDeletedEvent
    {
        // Metadata
        public string EventType { get; init; } = nameof(UserDeletedEvent);
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

        // Payload
        public Guid Id { get; set; }
    }
}
