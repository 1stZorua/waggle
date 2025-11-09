namespace Waggle.Common.Pagination.Models
{
    public class PaginationRequest
    {
        public string? Cursor { get; set; }
        public int PageSize { get; set; } = 20;
        public PaginationDirection Direction { get; set; } = PaginationDirection.Forward;

        public const int MaxPageSize = 100;
        public const int DefaultPageSize = 20;

        public void Validate()
        {
            if (PageSize <= 0)
                PageSize = DefaultPageSize;
            if (PageSize > MaxPageSize)
                PageSize = MaxPageSize;
        }
    }
}
