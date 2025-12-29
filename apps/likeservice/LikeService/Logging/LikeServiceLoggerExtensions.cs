namespace Waggle.LikeService.Logging
{
    public static partial class LikeServiceLoggerExtensions
    {
        // Like Retrieval Events

        [LoggerMessage(
            EventId = 1001,
            Level = LogLevel.Information,
            Message = "Retrieved all likes, count: {Count}")]
        public static partial void LogLikesRetrieved(
            this ILogger logger,
            int count);

        [LoggerMessage(
            EventId = 1002,
            Level = LogLevel.Error,
            Message = "Failed to retrieve all likes")]
        public static partial void LogLikesRetrievalFailed(
            this ILogger logger,
            Exception exception);

        [LoggerMessage(
            EventId = 1003,
            Level = LogLevel.Information,
            Message = "Retrieved like: {LikeId}")]
        public static partial void LogLikeRetrieved(
            this ILogger logger,
            Guid likeId);

        [LoggerMessage(
            EventId = 1004,
            Level = LogLevel.Warning,
            Message = "Like not found: {LikeId}")]
        public static partial void LogLikeNotFound(
            this ILogger logger,
            Guid likeId);

        [LoggerMessage(
            EventId = 1005,
            Level = LogLevel.Error,
            Message = "Failed to retrieve like: {LikeId}")]
        public static partial void LogLikeRetrievalFailed(
            this ILogger logger,
            Exception exception,
            Guid likeId);

        // Like Creation Events

        [LoggerMessage(
            EventId = 2001,
            Level = LogLevel.Information,
            Message = "Like created: {UserId} → ({LikeId})")]
        public static partial void LogLikeCreated(
            this ILogger logger,
            Guid userId,
            Guid likeId);

        [LoggerMessage(
            EventId = 2002,
            Level = LogLevel.Warning,
            Message = "Like target not found: {TargetType} {TargetId}")]
                public static partial void LogLikeTargetNotFound(
            this ILogger logger,
            string targetType,
            Guid targetId);

        [LoggerMessage(
            EventId = 2003,
            Level = LogLevel.Error,
            Message = "Database update failed creating like: {UserId}")]
        public static partial void LogDatabaseUpdateFailed(
            this ILogger logger,
            Exception exception,
            Guid userId);

        [LoggerMessage(
            EventId = 2004,
            Level = LogLevel.Error,
            Message = "Like creation failed: {UserId}")]
        public static partial void LogLikeCreationFailed(
            this ILogger logger,
            Exception exception,
            Guid userId);

        [LoggerMessage(
            EventId = 2005,
            Level = LogLevel.Warning,
            Message = "User {UserId} already liked this target: {LikeId}")]
        public static partial void LogLikeAlreadyExists(
            this ILogger logger,
            Guid likeId,
            Guid userId);

        // Like Deletion Events

        [LoggerMessage(
            EventId = 3001,
            Level = LogLevel.Information,
            Message = "Like deleted: {LikeId}")]
        public static partial void LogLikeDeleted(
            this ILogger logger,
            Guid likeId);

        [LoggerMessage(
            EventId = 3002,
            Level = LogLevel.Warning,
            Message = "Like not found for deletion: {LikeId}")]
        public static partial void LogLikeDeleteNotFound(
            this ILogger logger,
            Guid likeId);

        [LoggerMessage(
            EventId = 3003,
            Level = LogLevel.Error,
            Message = "Like deletion failed: {LikeId}")]
        public static partial void LogLikeDeletionFailed(
            this ILogger logger,
            Exception exception,
            Guid likeId);

        [LoggerMessage(
            EventId = 3004,
            Level = LogLevel.Warning,
            Message = "Unauthorized deletion attempt on like {LikeId} by user {UserId}")]
        public static partial void LogUnauthorizedDeleteAttempt(
            this ILogger logger,
            Guid userId,
            Guid likeId);

        // Event-Based Like Deletion Events

        [LoggerMessage(
            EventId = 4001,
            Level = LogLevel.Information,
            Message = "Like deleted from event: {LikeId}")]
        public static partial void LogLikeDeletedFromEvent(
            this ILogger logger,
            Guid likeId);

        [LoggerMessage(
            EventId = 4002,
            Level = LogLevel.Information,
            Message = "Like not found for deletion from event: {LikeId}")]
        public static partial void LogLikeDeleteNotFoundFromEvent(
            this ILogger logger,
            Guid likeId);

        [LoggerMessage(
            EventId = 4003,
            Level = LogLevel.Error,
            Message = "Like deletion from event failed: {LikeId}")]
        public static partial void LogLikeDeletionFromEventFailed(
            this ILogger logger,
            Exception exception,
            Guid likeId);
    }
}