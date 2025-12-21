namespace Waggle.AuthService.Logging
{
    public static partial class AuthServiceLoggerExtensions
    {
        // User Registration Events

        [LoggerMessage(
            EventId = 1001,
            Level = LogLevel.Information,
            Message = "User registered: {Username} ({UserId})")]
        public static partial void LogUserRegistered(
            this ILogger logger,
            string username,
            Guid userId);

        [LoggerMessage(
            EventId = 1002,
            Level = LogLevel.Error,
            Message = "Admin token acquisition failed for user: {Username} - {ErrorMessage}")]
        public static partial void LogAdminTokenFailed(
            this ILogger logger,
            string username,
            string? errorMessage);

        [LoggerMessage(
            EventId = 1003,
            Level = LogLevel.Error,
            Message = "Keycloak user creation failed: {Username} - {ErrorMessage} ({ErrorCode})")]
        public static partial void LogKeycloakUserCreationFailed(
            this ILogger logger,
            string username,
            string? errorMessage,
            string? errorCode);

        [LoggerMessage(
            EventId = 1004,
            Level = LogLevel.Error,
            Message = "Registered event publication failed: {Username} ({UserId})")]
        public static partial void LogRegisteredEventPublishFailed(
            this ILogger logger,
            Exception exception,
            string username,
            Guid userId);

        [LoggerMessage(
            EventId = 1004,
            Level = LogLevel.Error,
            Message = "Registration completed event publication failed: {Username} ({UserId})")]
        public static partial void LogRegistrationCompletedEventPublishFailed(
            this ILogger logger,
            Exception exception,
            string username,
            Guid userId);

        [LoggerMessage(
            EventId = 1005,
            Level = LogLevel.Error,
            Message = "User info retrieval failed: {Username} ({ErrorCode})")]
        public static partial void LogUserInfoRetrievalFailed(
            this ILogger logger,
            string username,
            string? errorCode);

        [LoggerMessage(
            EventId = 1006,
            Level = LogLevel.Warning,
            Message = "User authenticated with Keycloak but not found in UserService: {UserId}")]
        public static partial void LogUserNotFoundInUserService(
            this ILogger logger,
            Guid userId);

        [LoggerMessage(
            EventId = 1007,
            Level = LogLevel.Error,
            Message = "Keycloak user deletion failed: {UserId} - {ErrorMessage} ({ErrorCode})")]
        public static partial void LogKeycloakUserDeletionFailed(
            this ILogger logger,
            Guid userId,
            string? errorMessage,
            string? errorCode);

        [LoggerMessage(
            EventId = 1008,
            Level = LogLevel.Error,
            Message = "Deleted event publication failed: {UserId}")]
        public static partial void LogDeletedEventPublishFailed(
            this ILogger logger,
            Exception exception,
            Guid userId);

        [LoggerMessage(
            EventId = 1009,
            Level = LogLevel.Warning,
            Message = "Unauthorized deletion attempt on account {TargetUserId} by user {UserId}")]
        public static partial void LogUnauthorizedDeleteAttempt(
            this ILogger logger,
            Guid userId,
            Guid targetUserId);

        // Token Events

        [LoggerMessage(
            EventId = 2001,
            Level = LogLevel.Error,
            Message = "Token retrieval failed: {ErrorMessage}")]
        public static partial void LogTokenRetrievalFailed(
            this ILogger logger,
            string? errorMessage);

        [LoggerMessage(
            EventId = 2002,
            Level = LogLevel.Warning,
            Message = "Invalid bearer token format")]
        public static partial void LogInvalidBearerTokenFormat(
            this ILogger logger);

        [LoggerMessage(
            EventId = 2003,
            Level = LogLevel.Warning,
            Message = "Access token missing")]
        public static partial void LogMissingAccessToken(
            this ILogger logger);

        // Keycloak Communication Events

        [LoggerMessage(
            EventId = 3001,
            Level = LogLevel.Error,
            Message = "JSON deserialization failed for {RequestUri}")]
        public static partial void LogJsonParseError(
            this ILogger logger,
            Exception exception,
            string? requestUri);

        [LoggerMessage(
            EventId = 3002,
            Level = LogLevel.Error,
            Message = "HTTP request failed for {RequestUri}")]
        public static partial void LogKeycloakRequestException(
            this ILogger logger,
            Exception exception,
            string? requestUri);

        [LoggerMessage(
            EventId = 3003,
            Level = LogLevel.Error,
            Message = "Keycloak request failed for {RequestUri}")]
        public static partial void LogKeycloakRequestFailed(
            this ILogger logger,
            Exception exception,
            string? requestUri);

        [LoggerMessage(
            EventId = 3004,
            Level = LogLevel.Error,
            Message = "Location header missing in response from {RequestUri}")]
        public static partial void LogMissingLocationHeader(
            this ILogger logger,
            string? requestUri);
    }
}