namespace Waggle.AuthService.Dtos
{
    public class TokenResponseDto
    {
        public required string AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public required int ExpiresIn { get; set; }
        public int RefreshExpiresIn { get; set; }
        public required string TokenType { get; set; }
    }
}
