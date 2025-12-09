using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Waggle.PostService.Models
{
    [Table("posts")]
    public class Post
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public string Caption { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "jsonb")]
        public List<Guid> MediaIds { get; set; } = [];

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
