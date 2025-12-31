namespace Waggle.UserService.Dtos
{
    public class UrlResponseDto
    {
        public required string Url { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
