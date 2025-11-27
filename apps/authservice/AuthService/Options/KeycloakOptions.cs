namespace Waggle.AuthService.Options
{
    public class KeycloakOptions
    {
        public required string AuthServerUrl { get; set; }
        public required string Realm { get; set; }
        public required string ClientId { get; set; }
        public required string ClientSecret { get; set; }
        public required string AdminClientId { get; set; }
        public required string AdminClientSecret { get; set; }
    }
}
