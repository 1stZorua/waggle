namespace Waggle.AuthService.Constants
{
    public static class AuthValidationErrors
    {
        public static class Login
        {
            public const string IdentifierRequired = "Username or email is required";
            public const string EmailInvalid = "Email address is not valid";
            public const string PasswordRequired = "Password is required";
        }

        public static class Register {
            public const string UsernameRequired = "Username is required";
            public const string UsernameInvalid = "Username must be between {0}-{1} characters and contain only letters and numbers";

            public const string EmailRequired = "Email is required";
            public const string EmailInvalid = "Email address is not valid";

            public const string PasswordRequired = "Password is required";
            public const string PasswordTooShort = "Password must be at least {0} characters long";
            public const string PasswordTooLong = "Password must not exceed {0} characters";

            public const string ConfirmPasswordRequired = "Confirm password is required";
            public const string ConfirmPasswordMismatch = "Passwords do not match";

            public const string FirstNameRequired = "First name is required";
            public const string FirstNameInvalid = "First name must contain only letters";

            public const string LastNameRequired = "Last name is required";
            public const string LastNameInvalid = "Last name must contain only letters";
        }
    }
}