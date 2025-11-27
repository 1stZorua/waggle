using System.Security.Claims;

namespace Waggle.Common.Auth
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetClaimValue(this ClaimsPrincipal user, string claimType, bool required = false)
        {
            var value = user.FindFirst(claimType)?.Value;
            if (required && string.IsNullOrEmpty(value))
                throw new Exception($"Required claim '{claimType}' is missing.");
            return value ?? string.Empty;
        }

        public static UserInfoDto ToUserInfo(this ClaimsPrincipal user)
        {
            ArgumentNullException.ThrowIfNull(user);

            var realmAccess = user.FindFirst("realm_access")?.Value is { } json
                ? System.Text.Json.JsonSerializer.Deserialize<RealmAccessDto>(json)
                : null;

            return new UserInfoDto
            {
                Sub = user.GetClaimValue(ClaimTypes.NameIdentifier, required: true),
                Username = user.GetClaimValue("preferred_username"),
                Email = user.GetClaimValue(ClaimTypes.Email),
                Name = user.GetClaimValue(ClaimTypes.Name),
                RealmAccess = realmAccess
            };
        }
    }
}
