namespace Waggle.Common.Auth
{
    public static class UserInfoExtensions
    {
        /// <summary>
        /// Parses the Sub claim to a Guid.
        /// </summary>
        public static Guid GetUserId(this UserInfoDto user)
        {
            ArgumentNullException.ThrowIfNull(user);
            return Guid.Parse(user.Sub);
        }

        /// <summary>
        /// Checks if the user has the "admin" role.
        /// </summary>
        public static bool IsAdmin(this UserInfoDto user)
        {
            ArgumentNullException.ThrowIfNull(user);
            return user.Roles?.Any(r => r.Equals("admin", StringComparison.OrdinalIgnoreCase)) ?? false;
        }

        /// <summary>
        /// Checks if the user has a specific role.
        /// </summary>
        public static bool HasRole(this UserInfoDto user, string role)
        {
            ArgumentNullException.ThrowIfNull(user);
            return user.Roles?.Any(r => r.Equals(role, StringComparison.OrdinalIgnoreCase)) ?? false;
        }

        /// <summary>
        /// Checks if the user is the same as a given user id.
        /// </summary>
        public static bool IsSelf(this UserInfoDto user, Guid userId)
        {
            return user.GetUserId() == userId;
        }
    }
}