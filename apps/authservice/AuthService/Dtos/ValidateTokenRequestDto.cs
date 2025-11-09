namespace Waggle.AuthService.Dtos
{
    public class ValidateTokenRequestDto
    {
        public required string BearerToken { get; set; }
    }
}
