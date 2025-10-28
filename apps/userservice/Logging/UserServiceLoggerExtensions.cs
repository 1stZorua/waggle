namespace AuthService.Logging
{
    public static partial class UserServiceLoggerExtensions
    {
        // User Retrieval Events

        [LoggerMessage(
            EventId = 1001,
            Level = LogLevel.Information,
            Message = "Retrieved all users, count: {Count}")]
        public static partial void LogUsersRetrieved(
            this ILogger logger,
            int count);

        [LoggerMessage(
            EventId = 1002,
            Level = LogLevel.Error,
            Message = "Failed to retrieve all users")]
        public static partial void LogUsersRetrievalFailed(
            this ILogger logger,
            Exception exception);

        [LoggerMessage(
            EventId = 1003,
            Level = LogLevel.Information,
            Message = "Retrieved user: {UserId}")]
        public static partial void LogUserRetrieved(
            this ILogger logger,
            Guid userId);

        [LoggerMessage(
            EventId = 1004,
            Level = LogLevel.Warning,
            Message = "User not found: {UserId}")]
        public static partial void LogUserNotFound(
            this ILogger logger,
            Guid userId);

        [LoggerMessage(
            EventId = 1005,
            Level = LogLevel.Error,
            Message = "Failed to retrieve user: {UserId}")]
        public static partial void LogUserRetrievalFailed(
            this ILogger logger,
            Exception exception,
            Guid userId);

        // User Creation Events

        [LoggerMessage(
            EventId = 2001,
            Level = LogLevel.Information,
            Message = "User created: {Username} ({UserId})")]
        public static partial void LogUserCreated(
            this ILogger logger,
            string username,
            Guid userId);

        [LoggerMessage(
            EventId = 2002,
            Level = LogLevel.Warning,
            Message = "User already exists: {UserId}")]
        public static partial void LogUserAlreadyExists(
            this ILogger logger,
            Guid userId);

        [LoggerMessage(
            EventId = 2003,
            Level = LogLevel.Error,
            Message = "Duplicate key error creating user: {UserId}")]
        public static partial void LogDuplicateKeyError(
            this ILogger logger,
            Exception exception,
            Guid userId);

        [LoggerMessage(
            EventId = 2004,
            Level = LogLevel.Error,
            Message = "Database update failed creating user: {UserId}")]
        public static partial void LogDatabaseUpdateFailed(
            this ILogger logger,
            Exception exception,
            Guid userId);

        [LoggerMessage(
            EventId = 2005,
            Level = LogLevel.Error,
            Message = "User creation failed: {UserId}")]
        public static partial void LogUserCreationFailed(
            this ILogger logger,
            Exception exception,
            Guid userId);

        // Event-Based User Creation Events

        [LoggerMessage(
            EventId = 3001,
            Level = LogLevel.Information,
            Message = "User created from event: {Username} ({UserId})")]
        public static partial void LogUserCreatedFromEvent(
            this ILogger logger,
            string username,
            Guid userId);

        [LoggerMessage(
            EventId = 3002,
            Level = LogLevel.Information,
            Message = "User already exists from event, returning existing: {UserId}")]
        public static partial void LogUserExistsFromEvent(
            this ILogger logger,
            Guid userId);

        [LoggerMessage(
            EventId = 3003,
            Level = LogLevel.Error,
            Message = "Duplicate key error creating user from event: {UserId}")]
        public static partial void LogDuplicateKeyErrorFromEvent(
            this ILogger logger,
            Exception exception,
            Guid userId);

        [LoggerMessage(
            EventId = 3004,
            Level = LogLevel.Error,
            Message = "Database update failed creating user from event: {UserId}")]
        public static partial void LogDatabaseUpdateFailedFromEvent(
            this ILogger logger,
            Exception exception,
            Guid userId);

        [LoggerMessage(
            EventId = 3005,
            Level = LogLevel.Error,
            Message = "User creation from event failed: {UserId}")]
        public static partial void LogUserCreationFromEventFailed(
            this ILogger logger,
            Exception exception,
            Guid userId);
    }
}