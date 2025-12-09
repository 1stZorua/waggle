using Waggle.Common.Constants;
using Waggle.Common.Results.Core;

namespace Waggle.Common.Auth
{
    public static class UserInfoExtensions
    {
        /// <summary>
        /// Validates that the current user is not null.
        /// </summary>
        public static bool TryEnsure(this UserInfoDto? user, out Result result)
        {
            if (user == null)
            {
                result = Result.Fail(ErrorMessages.Authentication.CurrentUserRetrievalFailed, ErrorCodes.Forbidden);
                return false;
            }

            result = default!;
            return true;
        }

        /// <summary>
        /// Validates that the current user is not null.
        /// </summary>
        public static bool TryEnsure<T>(this UserInfoDto? user, out Result<T> result)
        {
            if (user == null)
            {
                result = Result<T>.Fail(ErrorMessages.Authentication.CurrentUserRetrievalFailed, ErrorCodes.Forbidden);
                return false;
            }

            result = default!;
            return true;
        }


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