namespace Waggle.Common.Pagination.Models
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public PageInfo PageInfo { get; set; } = new();
    }
}
