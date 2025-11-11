namespace Waggle.Contracts.Auth.Events
{
    public class RegisteredEvent
    {
        // Metadata
        public string EventType { get; init; } = nameof(RegisteredEvent);
        public DateTime CreatedAt {  get; init; } = DateTime.UtcNow;

        // Payload
        public Guid Id { get; set; }
        public string Username { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
    }
}
