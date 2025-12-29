using Waggle.Common.Constants;

namespace Waggle.FollowService.Constants
{
    public static class FollowErrors
    {
        public static class Follow
        {
            public const string NotFound = "Follow not found";
            public const string RetrievalFailed = "Failed to retrieve follow";
            public const string CreationFailed = "Failed to create follow";
            public const string UpdateFailed = "Failed to update follow";
            public const string DeletionFailed = "Failed to delete follow";
            public const string InvalidId = "Invalid follow identifier";

            public const string AlreadyExists = ErrorMessages.Resource.AlreadyExists;

            public const string UserNotFound = "User not found";
            public const string CannotFollowSelf = "Cannot follow yourself";
        }

        public static class Service
        {
            public const string Unavailable = ErrorMessages.Service.Unavailable;
            public const string Failed = ErrorMessages.Service.Failed;
        }
    }
}
