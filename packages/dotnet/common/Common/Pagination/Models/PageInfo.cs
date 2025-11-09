namespace Waggle.Common.Pagination.Models
{
    public class PageInfo
    {
        public string? NextCursor { get; set; }
        public string? PreviousCursor { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
        public int PageSize { get; set; }
    }
}
