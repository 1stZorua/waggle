namespace Waggle.AuthService.Dtos
{
    public class LoginRequestDto
    {
        public required string Identifier { get; set; }
        public required string Password { get; set; }
    }
}
