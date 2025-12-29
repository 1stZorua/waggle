namespace Waggle.FollowService.Logging
{
    public static partial class FollowServiceLoggerExtensions
    {
        // Follow Retrieval Events

        [LoggerMessage(
            EventId = 1001,
            Level = LogLevel.Information,
            Message = "Retrieved all follows, count: {Count}")]
        public static partial void LogFollowsRetrieved(
            this ILogger logger,
            int count);

        [LoggerMessage(
            EventId = 1002,
            Level = LogLevel.Error,
            Message = "Failed to retrieve all follows")]
        public static partial void LogFollowsRetrievalFailed(
            this ILogger logger,
            Exception exception);

        [LoggerMessage(
            EventId = 1003,
            Level = LogLevel.Information,
            Message = "Retrieved follow: {FollowId}")]
        public static partial void LogFollowRetrieved(
            this ILogger logger,
            Guid followId);

        [LoggerMessage(
            EventId = 1004,
            Level = LogLevel.Warning,
            Message = "Follow not found: {FollowId}")]
        public static partial void LogFollowNotFound(
            this ILogger logger,
            Guid followId);

        [LoggerMessage(
            EventId = 1005,
            Level = LogLevel.Error,
            Message = "Failed to retrieve follow: {FollowId}")]
        public static partial void LogFollowRetrievalFailed(
            this ILogger logger,
            Exception exception,
            Guid followId);

        // Follow Creation Events

        [LoggerMessage(
            EventId = 2001,
            Level = LogLevel.Information,
            Message = "Follow created: {FollowerId} → {FollowingId} ({FollowId})")]
        public static partial void LogFollowCreated(
            this ILogger logger,
            Guid followId,
            Guid followerId,
            Guid followingId);

        [LoggerMessage(
            EventId = 2002,
            Level = LogLevel.Warning,
            Message = "User not found for follow: {UserId}")]
        public static partial void LogFollowUserNotFound(
            this ILogger logger,
            Guid userId);

        [LoggerMessage(
            EventId = 2003,
            Level = LogLevel.Warning,
            Message = "User {UserId} cannot follow themselves")]
        public static partial void LogCannotFollowSelf(
            this ILogger logger,
            Guid userId);

        [LoggerMessage(
            EventId = 2004,
            Level = LogLevel.Error,
            Message = "Database update failed creating follow: {UserId}")]
        public static partial void LogDatabaseUpdateFailed(
            this ILogger logger,
            Exception exception,
            Guid userId);

        [LoggerMessage(
            EventId = 2005,
            Level = LogLevel.Error,
            Message = "Follow creation failed: {UserId}")]
        public static partial void LogFollowCreationFailed(
            this ILogger logger,
            Exception exception,
            Guid userId);

        [LoggerMessage(
            EventId = 2006,
            Level = LogLevel.Warning,
            Message = "User {UserId} already follows this user: {FollowId}")]
        public static partial void LogFollowAlreadyExists(
            this ILogger logger,
            Guid followId,
            Guid userId);

        // Follow Deletion Events

        [LoggerMessage(
            EventId = 3001,
            Level = LogLevel.Information,
            Message = "Follow deleted: {FollowId}")]
        public static partial void LogFollowDeleted(
            this ILogger logger,
            Guid followId);

        [LoggerMessage(
            EventId = 3002,
            Level = LogLevel.Warning,
            Message = "Follow not found for deletion: {FollowId}")]
        public static partial void LogFollowDeleteNotFound(
            this ILogger logger,
            Guid followId);

        [LoggerMessage(
            EventId = 3003,
            Level = LogLevel.Error,
            Message = "Follow deletion failed: {FollowId}")]
        public static partial void LogFollowDeletionFailed(
            this ILogger logger,
            Exception exception,
            Guid followId);

        [LoggerMessage(
            EventId = 3004,
            Level = LogLevel.Warning,
            Message = "Unauthorized deletion attempt on follow {FollowId} by user {UserId}")]
        public static partial void LogUnauthorizedDeleteAttempt(
            this ILogger logger,
            Guid userId,
            Guid followId);

        // Event-Based Follow Deletion Events

        [LoggerMessage(
            EventId = 4001,
            Level = LogLevel.Information,
            Message = "Follow deleted from event: {FollowId}")]
        public static partial void LogFollowDeletedFromEvent(
            this ILogger logger,
            Guid followId);

        [LoggerMessage(
            EventId = 4002,
            Level = LogLevel.Information,
            Message = "Follow not found for deletion from event: {FollowId}")]
        public static partial void LogFollowDeleteNotFoundFromEvent(
            this ILogger logger,
            Guid followId);

        [LoggerMessage(
            EventId = 4003,
            Level = LogLevel.Error,
            Message = "Follow deletion from event failed: {FollowId}")]
        public static partial void LogFollowDeletionFromEventFailed(
            this ILogger logger,
            Exception exception,
            Guid followId);
    }
}