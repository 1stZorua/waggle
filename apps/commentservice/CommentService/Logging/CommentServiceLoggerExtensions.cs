namespace Waggle.CommentService.Logging
{
    public static partial class CommentServiceLoggerExtensions
    {
        // Comment Retrieval Events

        [LoggerMessage(
            EventId = 1001,
            Level = LogLevel.Information,
            Message = "Retrieved all comments, count: {Count}")]
        public static partial void LogCommentsRetrieved(
            this ILogger logger,
            int count);

        [LoggerMessage(
            EventId = 1002,
            Level = LogLevel.Error,
            Message = "Failed to retrieve all comments")]
        public static partial void LogCommentsRetrievalFailed(
            this ILogger logger,
            Exception exception);

        [LoggerMessage(
            EventId = 1003,
            Level = LogLevel.Information,
            Message = "Retrieved comment: {CommentId}")]
        public static partial void LogCommentRetrieved(
            this ILogger logger,
            Guid commentId);

        [LoggerMessage(
            EventId = 1004,
            Level = LogLevel.Warning,
            Message = "Comment not found: {CommentId}")]
        public static partial void LogCommentNotFound(
            this ILogger logger,
            Guid commentId);

        [LoggerMessage(
            EventId = 1005,
            Level = LogLevel.Error,
            Message = "Failed to retrieve comment: {CommentId}")]
        public static partial void LogCommentRetrievalFailed(
            this ILogger logger,
            Exception exception,
            Guid commentId);

        // Comment Creation Events

        [LoggerMessage(
            EventId = 2001,
            Level = LogLevel.Information,
            Message = "Comment created: {CommentId} by user {UserId}")]
        public static partial void LogCommentCreated(
            this ILogger logger,
            Guid commentId,
            Guid userId);

        [LoggerMessage(
            EventId = 2002,
            Level = LogLevel.Warning,
            Message = "Post not found for comment: {PostId}")]
        public static partial void LogCommentPostNotFound(
            this ILogger logger,
            Guid postId);

        [LoggerMessage(
            EventId = 2003,
            Level = LogLevel.Warning,
            Message = "Parent not found: {ParentId}")]
        public static partial void LogCommentParentNotFound(
            this ILogger logger,
            Guid parentId);

        [LoggerMessage(
            EventId = 2004,
            Level = LogLevel.Warning,
            Message = "Parent {ParentId} does not belong to post {PostId}")]
        public static partial void LogCommentParentPostMismatch(
            this ILogger logger,
            Guid parentId,
            Guid postId);

        [LoggerMessage(
            EventId = 2005,
            Level = LogLevel.Error,
            Message = "Database update failed creating comment: {UserId}")]
        public static partial void LogDatabaseUpdateFailed(
            this ILogger logger,
            Exception exception,
            Guid userId);

        [LoggerMessage(
            EventId = 2006,
            Level = LogLevel.Error,
            Message = "Comment creation failed: {UserId}")]
        public static partial void LogCommentCreationFailed(
            this ILogger logger,
            Exception exception,
            Guid userId);

        // Comment Update Events

        [LoggerMessage(
            EventId = 3001,
            Level = LogLevel.Information,
            Message = "Comment updated: {CommentId}")]
        public static partial void LogCommentUpdated(
            this ILogger logger,
            Guid commentId);

        [LoggerMessage(
            EventId = 3002,
            Level = LogLevel.Warning,
            Message = "Comment {CommentId} does not belong to post {PostId}")]
        public static partial void LogCommentPostMismatch(
            this ILogger logger,
            Guid commentId,
            Guid postId);

        [LoggerMessage(
            EventId = 3003,
            Level = LogLevel.Warning,
            Message = "Unauthorized update attempt on comment {CommentId} by user {UserId}")]
        public static partial void LogUnauthorizedUpdateAttempt(
            this ILogger logger,
            Guid userId,
            Guid commentId);

        [LoggerMessage(
            EventId = 3004,
            Level = LogLevel.Error,
            Message = "Comment update failed: {CommentId}")]
        public static partial void LogCommentUpdateFailed(
            this ILogger logger,
            Exception exception,
            Guid commentId);

        // Comment Deletion Events

        [LoggerMessage(
            EventId = 4001,
            Level = LogLevel.Information,
            Message = "Comment deleted: {CommentId}")]
        public static partial void LogCommentDeleted(
            this ILogger logger,
            Guid commentId);

        [LoggerMessage(
            EventId = 4002,
            Level = LogLevel.Warning,
            Message = "Comment not found for deletion: {CommentId}")]
        public static partial void LogCommentDeleteNotFound(
            this ILogger logger,
            Guid commentId);

        [LoggerMessage(
            EventId = 4003,
            Level = LogLevel.Error,
            Message = "Comment deletion failed: {CommentId}")]
        public static partial void LogCommentDeletionFailed(
            this ILogger logger,
            Exception exception,
            Guid commentId);

        [LoggerMessage(
            EventId = 4004,
            Level = LogLevel.Warning,
            Message = "Unauthorized deletion attempt on comment {CommentId} by user {UserId}")]
        public static partial void LogUnauthorizedDeleteAttempt(
            this ILogger logger,
            Guid userId,
            Guid commentId);

        [LoggerMessage(
            EventId = 4005,
            Level = LogLevel.Error,
            Message = "Failed to publish CommentDeletedEvent for comment: {CommentId}")]
        public static partial void LogDeletedEventPublishFailed(
            this ILogger logger,
            Exception exception,
            Guid commentId);

        // Event-Based Comment Deletion Events

        [LoggerMessage(
            EventId = 5001,
            Level = LogLevel.Information,
            Message = "Comment deleted from event: {CommentId}")]
        public static partial void LogCommentDeletedFromEvent(
            this ILogger logger,
            Guid commentId);

        [LoggerMessage(
            EventId = 5002,
            Level = LogLevel.Information,
            Message = "Comment not found for deletion from event: {CommentId}")]
        public static partial void LogCommentDeleteNotFoundFromEvent(
            this ILogger logger,
            Guid commentId);

        [LoggerMessage(
            EventId = 5003,
            Level = LogLevel.Error,
            Message = "Comment deletion from event failed: {CommentId}")]
        public static partial void LogCommentDeletionFromEventFailed(
            this ILogger logger,
            Exception exception,
            Guid commentId);
    }
}