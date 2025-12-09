using System.ComponentModel.DataAnnotations;

namespace Waggle.PostService.Dtos
{
    public class PostCreateDto
    {
        public required string Caption { get; set; }
        public required List<Guid> MediaIds { get; set; }
    }
}
