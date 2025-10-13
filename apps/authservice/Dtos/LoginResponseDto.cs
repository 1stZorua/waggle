namespace AuthService.Dtos
{
    public class LoginResponseDto
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
        public required int ExpiresIn { get; set; }
        public required string TokenType { get; set; }
    }
}
