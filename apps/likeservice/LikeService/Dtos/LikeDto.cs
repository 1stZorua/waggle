using Waggle.Common.Models;

namespace Waggle.LikeService.Dtos
{
    public class LikeDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid TargetId { get; set; }
        public InteractionType TargetType { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
