using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Waggle.Common.Models;

namespace Waggle.LikeService.Models
{
    [Table("likes")]
    public class Like
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid TargetId { get; set; }

        [Required]
        [Column(TypeName = "varchar(20)")]
        public InteractionType TargetType { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
