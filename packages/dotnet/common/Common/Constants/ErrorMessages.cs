namespace Waggle.Common.Constants
{
    public static class ErrorMessages
    {
        public static class Service
        {
            public const string Unavailable = "Service is currently unavailable";
            public const string Failed = "An unexpected error occured";
            public const string Timeout = "Request timed out";
        }

        public static class Validation
        {
            public const string Required = "This field is required";
            public const string InvalidFormat = "Invalid format provided";
            public const string InvalidInput = "The provided input is invalid";
        }

        public static class Authentication
        {
            public const string Unauthorized = "Authentication required";
            public const string Forbidden = "Access denied";
            public const string PermissionRequired = "You do not have the required permission";
            public const string TokenExpired = "Your session has expired";
            public const string InvalidToken = "Invalid authentication token";
        }

        public static class Resource
        {
            public const string NotFound = "The requested resource was not found";
            public const string AlreadyExists = "Resource already exists";
            public const string CreationFailed = "Unable to create resource";
        }
    }
}
