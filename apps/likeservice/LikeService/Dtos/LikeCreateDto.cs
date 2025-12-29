using Waggle.Common.Models;

namespace Waggle.LikeService.Dtos
{
    public class LikeCreateDto
    {
        public Guid TargetId { get; set; }
        public InteractionType TargetType { get; set; }
    }
}
