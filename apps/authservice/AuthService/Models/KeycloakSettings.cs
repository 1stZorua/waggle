namespace Waggle.AuthService.Models
{
    public class KeycloakSettings
    {
        public required string AuthServerUrl { get; set; }
        public required string Realm { get; set; }
        public required string ClientId { get; set; }
        public required string ClientSecret { get; set; }
        public required string AdminClientId { get; set; }
        public required string AdminClientSecret { get; set; }
    }
}
