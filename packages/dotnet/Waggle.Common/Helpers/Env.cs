namespace Waggle.Common.Helpers
{
    public class Env
    {
        public static string GetRequired(string key)
        {
            return Environment.GetEnvironmentVariable(key)
                ?? throw new InvalidOperationException($"{key} environment variable is not set");
        }

        public static string? GetOptional(string key) => Environment.GetEnvironmentVariable(key);
    }
}
