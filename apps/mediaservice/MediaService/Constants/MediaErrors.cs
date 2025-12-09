using Waggle.Common.Constants;

namespace Waggle.MediaService.Constants
{
    public static class MediaErrors
    {
        public static class Media
        {
            public const string NotFound = "Media not found";
            public const string RetrievalFailed = "Failed to retrieve media";
            public const string CreationFailed = "Failed to create media";
            public const string UploadFailed = "Failed to upload media";
            public const string GenerateUrlFailed = "Failed to generate file URL";
            public const string UpdateFailed = "Failed to update media";
            public const string DeletionFailed = "Failed to delete media";
            public const string StorageDeletionFailed = "Failed to delete media in storage";
            public const string InvalidId = "Invalid media identifier";

            public const string AlreadyExists = ErrorMessages.Resource.AlreadyExists;
        }

        public static class Service
        {
            public const string Unavailable = ErrorMessages.Service.Unavailable;
            public const string Failed = ErrorMessages.Service.Failed;
        }
    }
}
