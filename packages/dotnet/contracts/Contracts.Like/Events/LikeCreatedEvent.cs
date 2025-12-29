using Waggle.Common.Models;

namespace Waggle.Contracts.Like.Events
{
    public class LikeCreatedEvent
    {
        // Metadata
        public string EventType { get; init; } = nameof(LikeCreatedEvent);
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

        // Payload
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid TargetId { get; set; }
        public InteractionType TargetType {get; set; }
    }
}
