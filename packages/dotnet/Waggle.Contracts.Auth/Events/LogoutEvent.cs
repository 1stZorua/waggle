namespace Waggle.Contracts.Auth.Events
{
    public class LogoutEvent
    {
        // Metadata
        public string EventType { get; init; } = nameof(LogoutEvent);
        public DateTime CreatedAt {  get; init; } = DateTime.UtcNow;

        // Payload
        public Guid UserId { get; set; }
        public string UserName { get; init; } = string.Empty;
    }
}
