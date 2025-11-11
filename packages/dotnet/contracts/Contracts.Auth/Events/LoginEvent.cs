namespace Waggle.Contracts.Auth.Events
{
    public class LoginEvent
    {
        // Metadata
        public string EventType { get; init; } = nameof(LoginEvent);
        public DateTime CreatedAt {  get; init; } = DateTime.UtcNow;

        // Payload
        public Guid Id { get; set; }
        public string UserName { get; init; } = string.Empty;
    }
}
