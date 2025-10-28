namespace AuthService.Logging
{
    public static partial class HealthServiceLoggerExtensions
    {
        [LoggerMessage(
            EventId = 4001,
            Level = LogLevel.Warning,
            Message = "Keycloak health check failed with status {StatusCode} for {HealthUrl}")]
        public static partial void LogKeycloakHealthCheckFailed(
            this ILogger logger,
            System.Net.HttpStatusCode statusCode,
            string healthUrl);

        [LoggerMessage(
            EventId = 4002,
            Level = LogLevel.Error,
            Message = "Keycloak health check threw exception for {HealthUrl}")]
        public static partial void LogKeycloakHealthCheckException(
            this ILogger logger,
            Exception exception,
            string healthUrl);
    }
}