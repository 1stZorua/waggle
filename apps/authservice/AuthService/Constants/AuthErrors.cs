using Waggle.Common.Constants;

namespace Waggle.AuthService.Constants
{
    public static class AuthErrors
    {
        public static class Token
        {
            public const string AdminAccessFailed = "Failed to obtain administrative access";
            public const string RetrievalFailed = "Failed to retrieve authentication token";
            public const string InvalidFormat = "Authorization header must use Bearer token format";
            public const string Missing = "Authentication token is required";
            public const string InvalidCredentials = "Incorrect username or password";

            public const string Invalid = ErrorMessages.Authentication.InvalidToken;
            public const string Expired = ErrorMessages.Authentication.TokenExpired;
        }

        public static class User
        {
            public const string CreationFailed = "Failed to create user account";
            public const string ProfileInitFailed = "Account created but profile initialization failed";
            public const string DeletionFailed = "Failed to remove user from authentication system";
            public const string InfoRetrievalFailed = "Failed to retrieve user information";
            public const string AlreadyExists = "An account with this email already exists";
            public const string InvalidId = "Incorrect user identifier";
        }

        public static class Service
        {
            public const string Unavailable = "Authentication service is currently unavailable";

            public const string Failed = ErrorMessages.Service.Failed;
        }

        public static class Session
        {
            public const string EndFailed = "Failed to end user session";
        }

        public static class Response
        {
            public const string MissingLocation = "Resource created but identifier not returned";
            public const string ParseFailed = "Failed to process server response";
        }
    }
}
