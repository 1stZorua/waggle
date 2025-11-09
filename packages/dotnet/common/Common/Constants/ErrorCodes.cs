namespace Waggle.Common.Constants
{
    public static class ErrorCodes
    {
        // Authentication & Authorization
        public const string Unauthorized = "UNAUTHORIZED";
        public const string Forbidden = "FORBIDDEN";
        public const string TokenExpired = "TOKEN_EXPIRED";
        public const string InvalidCredentials = "INVALID_CREDENTIALS";

        // Validation
        public const string ValidationFailed = "VALIDATION_FAILED";
        public const string InvalidInput = "INVALID_INPUT";

        // Resources
        public const string NotFound = "NOT_FOUND";
        public const string AlreadyExists = "ALREADY_EXISTS";

        // Services
        public const string ServiceFailed = "SERVICE_FAILED";
        public const string ServiceUnavailable = "SERVICE_UNAVAILABLE";
        public const string Timeout = "TIMEOUT";
    }
}
