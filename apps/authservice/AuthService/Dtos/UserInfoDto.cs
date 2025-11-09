using System.Text.Json.Serialization;

namespace Waggle.AuthService.Dtos
{
    public class UserInfoDto
    {
        [JsonPropertyName("sub")]
        public required string Sub { get; set; }

        [JsonPropertyName("preferred_username")]
        public required string Username { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        public string[]? Roles { get; set; }
    }
}
