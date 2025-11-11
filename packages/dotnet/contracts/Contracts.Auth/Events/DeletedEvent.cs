namespace Waggle.Contracts.Auth.Events
{
    public class DeletedEvent
    {
        // Metadata
        public string EventType { get; init; } = nameof(DeletedEvent);
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

        // Payload
        public Guid Id { get; set; }
    }
}
