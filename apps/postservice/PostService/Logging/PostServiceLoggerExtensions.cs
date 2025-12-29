namespace Waggle.PostService.Logging
{
    public static partial class PostServiceloggerExtensions
    {
        // Post Retrieval Events

        [LoggerMessage(
            EventId = 1001,
            Level = LogLevel.Information,
            Message = "Retrieved all posts, count: {Count}")]
        public static partial void LogPostsRetrieved(
            this ILogger logger,
            int count);

        [LoggerMessage(
            EventId = 1002,
            Level = LogLevel.Error,
            Message = "Failed to retrieve all posts")]
        public static partial void LogPostsRetrievalFailed(
            this ILogger logger,
            Exception exception);

        [LoggerMessage(
            EventId = 1003,
            Level = LogLevel.Information,
            Message = "Retrieved post: {PostId}")]
        public static partial void LogPostRetrieved(
            this ILogger logger,
            Guid postId);

        [LoggerMessage(
            EventId = 1004,
            Level = LogLevel.Warning,
            Message = "Post not found: {PostId}")]
        public static partial void LogPostNotFound(
            this ILogger logger,
            Guid postId);

        [LoggerMessage(
            EventId = 1005,
            Level = LogLevel.Error,
            Message = "Failed to retrieve post: {PostId}")]
        public static partial void LogPostRetrievalFailed(
            this ILogger logger,
            Exception exception,
            Guid postId);

        // Post Creation Events

        [LoggerMessage(
            EventId = 2001,
            Level = LogLevel.Information,
            Message = "Post created: {UserId} → ({PostId})")]
        public static partial void LogPostCreated(
            this ILogger logger,
            Guid userId,
            Guid postId);

        [LoggerMessage(
            EventId = 2004,
            Level = LogLevel.Error,
            Message = "Database update failed creating post: {UserId}")]
        public static partial void LogDatabaseUpdateFailed(
            this ILogger logger,
            Exception exception,
            Guid userId);

        [LoggerMessage(
            EventId = 2005,
            Level = LogLevel.Error,
            Message = "Post creation failed: {UserId}")]
        public static partial void LogPostCreationFailed(
            this ILogger logger,
            Exception exception,
            Guid userId);

        [LoggerMessage(
            EventId = 2006,
            Level = LogLevel.Warning,
            Message = "Post creation failed due to missing media: {UserId}")]
        public static partial void LogPostCreationFailedMedia(
            this ILogger logger,
            Guid userId);

        // Post Deletion Events

        [LoggerMessage(
            EventId = 3001,
            Level = LogLevel.Information,
            Message = "Post deleted: {PostId}")]
        public static partial void LogPostDeleted(
            this ILogger logger,
            Guid postId);

        [LoggerMessage(
            EventId = 3002,
            Level = LogLevel.Warning,
            Message = "Post not found for deletion: {PostId}")]
        public static partial void LogPostDeleteNotFound(
            this ILogger logger,
            Guid postId);

        [LoggerMessage(
            EventId = 3003,
            Level = LogLevel.Error,
            Message = "Post deletion failed: {PostId}")]
        public static partial void LogPostDeletionFailed(
            this ILogger logger,
            Exception exception,
            Guid postId);

        [LoggerMessage(
            EventId = 3004,
            Level = LogLevel.Warning,
            Message = "Unauthorized deletion attempt on post {PostId} by user {UserId}")]
        public static partial void LogUnauthorizedDeleteAttempt(
            this ILogger logger,
            Guid userId,
            Guid postId);

        [LoggerMessage(
            EventId = 1008,
            Level = LogLevel.Error,
            Message = "Deleted event publication failed: {PostId}")]
        public static partial void LogDeletedEventPublishFailed(
            this ILogger logger,
            Exception exception,
            Guid postId);

        // Event-Based Post Deletion Events

        [LoggerMessage(
            EventId = 4001,
            Level = LogLevel.Information,
            Message = "Post deleted from event: {PostId}")]
        public static partial void LogPostDeletedFromEvent(
            this ILogger logger,
            Guid postId);

        [LoggerMessage(
            EventId = 4002,
            Level = LogLevel.Information,
            Message = "Post not found for deletion from event: {PostId}")]
        public static partial void LogPostDeleteNotFoundFromEvent(
            this ILogger logger,
            Guid postId);

        [LoggerMessage(
            EventId = 4003,
            Level = LogLevel.Error,
            Message = "Post deletion from event failed: {PostId}")]
        public static partial void LogPostDeletionFromEventFailed(
            this ILogger logger,
            Exception exception,
            Guid postId);
    }
}