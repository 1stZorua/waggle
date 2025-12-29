using Waggle.Common.Models;

namespace Waggle.FavoriteService.Dtos
{
    public class FavoriteCreateDto
    {
        public Guid TargetId { get; set; }
        public InteractionType TargetType { get; set; }
    }
}
