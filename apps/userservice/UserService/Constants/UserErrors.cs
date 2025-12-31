using Waggle.Common.Constants;

namespace Waggle.UserService.Constants
{
    public static class UserErrors
    {
        public static class User
        {
            public const string NotFound = "User not found";
            public const string RetrievalFailed = "Failed to retrieve user";
            public const string CreationFailed = "Failed to create user";
            public const string UpdateFailed = "Failed to update user";
            public const string DeletionFailed = "Failed to delete user";
            public const string InvalidId = "Invalid user identifier";

            public const string AlreadyExists = ErrorMessages.Resource.AlreadyExists;

            public const string MediaDoesNotExist = "Media item does not exist";
        }

        public static class Service
        {
            public const string Unavailable = ErrorMessages.Service.Unavailable;
            public const string Failed = ErrorMessages.Service.Failed;
        }
    }
}
