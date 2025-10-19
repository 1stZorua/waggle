namespace Waggle.Common.Models
{
    public enum ApiStatus
    {
        Success,
        Fail,
        Error
    }

    public class ApiResponse
    {
        public required ApiStatus Status { get; set; }
        public object? Data { get; set; }
        public string? Message { get; set; }
        public string? Code { get; set; }
    }

    public class ApiResponse<T> : ApiResponse
    {
        public new T? Data { get; set; }
    }
}