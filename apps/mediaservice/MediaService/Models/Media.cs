using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Waggle.MediaService.Models
{
    [Table("media")]
    public class Media
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid UploaderId { get; set; }

        [Required]
        [MaxLength(63)]
        public required string BucketName { get; set; }

        [Required]
        [MaxLength(1024)]
        public required string ObjectName { get; set; }

        [Required]
        [MaxLength(255)]
        public required string FileName { get; set; }

        [Required]
        [MaxLength(100)]
        public required string ContentType { get; set; }

        public long FileSize { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
