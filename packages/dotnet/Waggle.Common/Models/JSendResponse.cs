namespace Waggle.Common.Models
{
    public enum JSendStatus
    {
        Success,
        Fail,
        Error
    }

    public class JSendResponse
    {
        public required JSendStatus Status { get; set; }
        public object? Data { get; set; }
        public string? Message { get; set; }
        public string? Code { get; set; }
    }

    public class JSendResponse<T> : JSendResponse
    {
        public new T? Data { get; set; }
    }
}