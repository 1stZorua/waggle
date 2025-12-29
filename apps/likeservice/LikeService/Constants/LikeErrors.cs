using Waggle.Common.Constants;

namespace Waggle.LikeService.Constants
{
    public static class LikeErrors
    {
        public static class Like
        {
            public const string NotFound = "Like not found";
            public const string RetrievalFailed = "Failed to retrieve like";
            public const string CreationFailed = "Failed to create like";
            public const string UpdateFailed = "Failed to update like";
            public const string DeletionFailed = "Failed to delete like";
            public const string InvalidId = "Invalid like identifier";

            public const string AlreadyExists = ErrorMessages.Resource.AlreadyExists;

            public const string TargetNotFound = "Target does not exist";
            public const string InvalidTarget = "Invalid target";
        }

        public static class Service
        {
            public const string Unavailable = ErrorMessages.Service.Unavailable;
            public const string Failed = ErrorMessages.Service.Failed;
        }
    }
}
