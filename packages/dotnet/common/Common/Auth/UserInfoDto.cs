using System.Text.Json.Serialization;

namespace Waggle.Common.Auth
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

        [JsonPropertyName("realm_access")]
        public RealmAccessDto? RealmAccess { get; set; }

        [JsonIgnore]
        public string[]? Roles
        {
            get => RealmAccess?.Roles;
            set
            {
                if (RealmAccess == null) RealmAccess = new RealmAccessDto();
                RealmAccess.Roles = value;
            }
        }
    }

    public class RealmAccessDto
    {
        [JsonPropertyName("roles")]
        public string[]? Roles { get; set; }
    }

}
