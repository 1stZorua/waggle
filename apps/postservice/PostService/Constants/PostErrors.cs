using Waggle.Common.Constants;

namespace Waggle.PostService.Constants
{
    public static class PostErrors
    {
        public static class Post
        {
            public const string NotFound = "Post not found";
            public const string RetrievalFailed = "Failed to retrieve post";
            public const string CreationFailed = "Failed to create post";
            public const string UpdateFailed = "Failed to update post";
            public const string DeletionFailed = "Failed to delete post";
            public const string InvalidId = "Invalid post identifier";

            public const string AlreadyExists = ErrorMessages.Resource.AlreadyExists;

            public const string MediaDoesNotExist = "One or more media items do not exist";
        }

        public static class Service
        {
            public const string Unavailable = ErrorMessages.Service.Unavailable;
            public const string Failed = ErrorMessages.Service.Failed;
        }
    }
}
