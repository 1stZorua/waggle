using System.ComponentModel.DataAnnotations;

namespace UserService.Dtos
{
    public class UserCreateDto
    {
        [Required]
        public required string Name { get; set; }

        [Required]
        public required string Username { get; set; }
    }
}
