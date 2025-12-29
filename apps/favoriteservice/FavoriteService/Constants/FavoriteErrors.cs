using Waggle.Common.Constants;

namespace Waggle.FavoriteService.Constants
{
    public static class FavoriteErrors
    {
        public static class Favorite
        {
            public const string NotFound = "Favorite not found";
            public const string RetrievalFailed = "Failed to retrieve favorite";
            public const string CreationFailed = "Failed to create favorite";
            public const string UpdateFailed = "Failed to update favorite";
            public const string DeletionFailed = "Failed to delete favorite";
            public const string InvalidId = "Invalid favorite identifier";

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
