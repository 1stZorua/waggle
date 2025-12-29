using Waggle.Common.Constants;

namespace Waggle.CommentService.Constants
{
    public static class CommentErrors
    {
        public static class Comment
        {
            public const string NotFound = "Comment not found";
            public const string RetrievalFailed = "Failed to retrieve comment";
            public const string CreationFailed = "Failed to create comment";
            public const string UpdateFailed = "Failed to update comment";
            public const string DeletionFailed = "Failed to delete comment";
            public const string InvalidId = "Invalid comment identifier";

            public const string AlreadyExists = ErrorMessages.Resource.AlreadyExists;

            public const string PostNotFound = "Post not found";
            public const string ParentCommentNotFound = "Parent comment not found";
        }

        public static class Service
        {
            public const string Unavailable = ErrorMessages.Service.Unavailable;
            public const string Failed = ErrorMessages.Service.Failed;
        }
    }
}
