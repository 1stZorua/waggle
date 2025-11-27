namespace Waggle.AuthService.Options
{
    public class AuthOptions
    {
        // Username
        public required int MinUsernameLength { get; set; }
        public required int MaxUsernameLength { get; set; }

        // Password
        public required int MinPasswordLength { get; set; }
        public required int MaxPasswordLength { get; set; }
    }
}
