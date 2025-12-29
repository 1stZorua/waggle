using Waggle.Common.Models;

namespace Waggle.FavoriteService.Dtos
{
    public class FavoriteDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid TargetId { get; set; }
        public InteractionType TargetType { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
