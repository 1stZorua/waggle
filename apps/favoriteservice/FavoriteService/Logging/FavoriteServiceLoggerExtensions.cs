namespace Waggle.FavoriteService.Logging
{
    public static partial class FavoriteServiceLoggerExtensions
    {
        // Favorite Retrieval Events

        [LoggerMessage(
            EventId = 1001,
            Level = LogLevel.Information,
            Message = "Retrieved all favorites, count: {Count}")]
        public static partial void LogFavoritesRetrieved(
            this ILogger logger,
            int count);

        [LoggerMessage(
            EventId = 1002,
            Level = LogLevel.Error,
            Message = "Failed to retrieve all favorites")]
        public static partial void LogFavoritesRetrievalFailed(
            this ILogger logger,
            Exception exception);

        [LoggerMessage(
            EventId = 1003,
            Level = LogLevel.Information,
            Message = "Retrieved favorite: {FavoriteId}")]
        public static partial void LogFavoriteRetrieved(
            this ILogger logger,
            Guid favoriteId);

        [LoggerMessage(
            EventId = 1004,
            Level = LogLevel.Warning,
            Message = "Favorite not found: {FavoriteId}")]
        public static partial void LogFavoriteNotFound(
            this ILogger logger,
            Guid favoriteId);

        [LoggerMessage(
            EventId = 1005,
            Level = LogLevel.Error,
            Message = "Failed to retrieve favorite: {FavoriteId}")]
        public static partial void LogFavoriteRetrievalFailed(
            this ILogger logger,
            Exception exception,
            Guid favoriteId);

        // Favorite Creation Events

        [LoggerMessage(
            EventId = 2001,
            Level = LogLevel.Information,
            Message = "Favorite created: {UserId} → ({FavoriteId})")]
        public static partial void LogFavoriteCreated(
            this ILogger logger,
            Guid userId,
            Guid favoriteId);

        [LoggerMessage(
            EventId = 2002,
            Level = LogLevel.Warning,
            Message = "Favorite target not found: {TargetType} {TargetId}")]
        public static partial void LogFavoriteTargetNotFound(
            this ILogger logger,
            string targetType,
            Guid targetId);

        [LoggerMessage(
            EventId = 2003,
            Level = LogLevel.Error,
            Message = "Database update failed creating favorite: {UserId}")]
        public static partial void LogDatabaseUpdateFailed(
            this ILogger logger,
            Exception exception,
            Guid userId);

        [LoggerMessage(
            EventId = 2004,
            Level = LogLevel.Error,
            Message = "Favorite creation failed: {UserId}")]
        public static partial void LogFavoriteCreationFailed(
            this ILogger logger,
            Exception exception,
            Guid userId);

        [LoggerMessage(
            EventId = 2005,
            Level = LogLevel.Warning,
            Message = "User {UserId} already favorited this target: {FavoriteId}")]
        public static partial void LogFavoriteAlreadyExists(
            this ILogger logger,
            Guid favoriteId,
            Guid userId);

        // Favorite Deletion Events

        [LoggerMessage(
            EventId = 3001,
            Level = LogLevel.Information,
            Message = "Favorite deleted: {FavoriteId}")]
        public static partial void LogFavoriteDeleted(
            this ILogger logger,
            Guid favoriteId);

        [LoggerMessage(
            EventId = 3002,
            Level = LogLevel.Warning,
            Message = "Favorite not found for deletion: {FavoriteId}")]
        public static partial void LogFavoriteDeleteNotFound(
            this ILogger logger,
            Guid favoriteId);

        [LoggerMessage(
            EventId = 3003,
            Level = LogLevel.Error,
            Message = "Favorite deletion failed: {FavoriteId}")]
        public static partial void LogFavoriteDeletionFailed(
            this ILogger logger,
            Exception exception,
            Guid favoriteId);

        [LoggerMessage(
            EventId = 3004,
            Level = LogLevel.Warning,
            Message = "Unauthorized deletion attempt on favorite {FavoriteId} by user {UserId}")]
        public static partial void LogUnauthorizedDeleteAttempt(
            this ILogger logger,
            Guid userId,
            Guid favoriteId);

        // Event-Based Favorite Deletion Events

        [LoggerMessage(
            EventId = 4001,
            Level = LogLevel.Information,
            Message = "Favorite deleted from event: {FavoriteId}")]
        public static partial void LogFavoriteDeletedFromEvent(
            this ILogger logger,
            Guid favoriteId);

        [LoggerMessage(
            EventId = 4002,
            Level = LogLevel.Information,
            Message = "Favorite not found for deletion from event: {FavoriteId}")]
        public static partial void LogFavoriteDeleteNotFoundFromEvent(
            this ILogger logger,
            Guid favoriteId);

        [LoggerMessage(
            EventId = 4003,
            Level = LogLevel.Error,
            Message = "Favorite deletion from event failed: {FavoriteId}")]
        public static partial void LogFavoriteDeletionFromEventFailed(
            this ILogger logger,
            Exception exception,
            Guid favoriteId);
    }
}