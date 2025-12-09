namespace Waggle.Mediaervice.Logging
{
    public static partial class MediaServiceLoggerExtensions
    {
        // Media Retrieval Events

        [LoggerMessage(
            EventId = 1001,
            Level = LogLevel.Information,
            Message = "Retrieved all media, count: {Count}")]
        public static partial void LogMediaRetrieved(
            this ILogger logger,
            int count);

        [LoggerMessage(
            EventId = 1002,
            Level = LogLevel.Error,
            Message = "Failed to retrieve all media")]
        public static partial void LogAllMediaRetrievalFailed(
            this ILogger logger,
            Exception exception);

        [LoggerMessage(
            EventId = 1003,
            Level = LogLevel.Information,
            Message = "Retrieved media: {MediaId}")]
        public static partial void LogMediaRetrieved(
            this ILogger logger,
            Guid mediaId);

        [LoggerMessage(
            EventId = 1004,
            Level = LogLevel.Warning,
            Message = "Media not found: {MediaId}")]
        public static partial void LogMediaNotFound(
            this ILogger logger,
            Guid mediaId);

        [LoggerMessage(
            EventId = 1005,
            Level = LogLevel.Error,
            Message = "Failed to retrieve media: {MediaId}")]
        public static partial void LogMediaRetrievalFailed(
            this ILogger logger,
            Exception exception,
            Guid mediaId);

        [LoggerMessage(
            EventId = 1006,
            Level = LogLevel.Error,
            Message = "Failed to retrieve users batch")]
        public static partial void LogMediaBatchRetrievalFailed(
            this ILogger logger,
            Exception exception);

        // Media Creation Events

        [LoggerMessage(
            EventId = 2001,
            Level = LogLevel.Information,
            Message = "Media created: {UserId} → ({MediaId})")]
        public static partial void LogMediaUploaded(
            this ILogger logger,
            Guid userId,
            Guid mediaId);

        [LoggerMessage(
            EventId = 2004,
            Level = LogLevel.Error,
            Message = "Database update failed creating media: {UserId}")]
        public static partial void LogDatabaseUpdateFailed(
            this ILogger logger,
            Exception exception,
            Guid userId);

        [LoggerMessage(
            EventId = 2005,
            Level = LogLevel.Error,
            Message = "Media creation failed: {UserId}")]
        public static partial void LogMediaCreationFailed(
            this ILogger logger,
            Exception exception,
            Guid userId);

        // Media Deletion Events

        [LoggerMessage(
            EventId = 3001,
            Level = LogLevel.Information,
            Message = "Media deleted: {MediaId}")]
        public static partial void LogMediaDeleted(
            this ILogger logger,
            Guid mediaId);

        [LoggerMessage(
            EventId = 3002,
            Level = LogLevel.Warning,
            Message = "Media not found for deletion: {MediaId}")]
        public static partial void LogMediaDeleteNotFound(
            this ILogger logger,
            Guid mediaId);

        [LoggerMessage(
            EventId = 3003,
            Level = LogLevel.Error,
            Message = "Media deletion failed: {MediaId}")]
        public static partial void LogMediaDeletionFailed(
            this ILogger logger,
            Exception exception,
            Guid mediaId);

        [LoggerMessage(
            EventId = 3004,
            Level = LogLevel.Warning,
            Message = "Unauthorized deletion attempt on media {MediaId} by user {UserId}")]
        public static partial void LogUnauthorizedDeleteAttempt(
            this ILogger logger,
            Guid userId,
            Guid mediaId);
    }
}