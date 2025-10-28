namespace AuthService.Logging
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
            Message = "Admin token acquisition failed: {Username} - {ErrorMessage}")]
        public static partial void LogAdminTokenFailed(
            this ILogger logger,
            string username,
            string? errorMessage);

        [LoggerMessage(
            EventId = 1003,
            Level = LogLevel.Error,
            Message = "User creation failed: {Username} - {ErrorMessage} ({ErrorCode})")]
        public static partial void LogKeycloakUserCreationFailed(
            this ILogger logger,
            string username,
            string? errorMessage,
            string? errorCode);

        [LoggerMessage(
            EventId = 1004,
            Level = LogLevel.Error,
            Message = "Event publication failed: {Username} ({UserId})")]
        public static partial void LogRegisteredEventPublishFailed(
            this ILogger logger,
            Exception exception,
            string username,
            Guid userId);

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