using System.ComponentModel.DataAnnotations;

namespace Waggle.UserService.Dtos
{
    public class UserCreateDto
    {
        [Required]
        public required Guid Id { get; set; } 

        [Required]
        [StringLength(30, MinimumLength = 3)]
        public required string Username { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1)]
        public required string FirstName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1)]
        public required string LastName { get; set; }
    }
}
