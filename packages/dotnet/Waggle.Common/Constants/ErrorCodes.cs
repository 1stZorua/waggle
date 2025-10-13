namespace Waggle.Common.Constants
{
    public static class ErrorCodes
    {
        // Authentication & Authorization
        public const string Unauthorized = "ERR_UNAUTHORIZED";
        public const string Forbidden = "ERR_FORBIDDEN";

        // Validation
        public const string ValidationFailed = "ERR_VALIDATION_FAILED";
        public const string InvalidInput = "ERR_INVALID_INPUT";

        // Resources
        public const string NotFound = "ERR_NOT_FOUND";
        public const string AlreadyExists = "ERR_ALREADY_EXISTS";

        // Services
        public const string ServiceFailed = "ERR_SERVICE_FAILED";
        public const string ServiceUnavailable = "ERR_SERVICE_UNAVAILABLE";
    }
}
