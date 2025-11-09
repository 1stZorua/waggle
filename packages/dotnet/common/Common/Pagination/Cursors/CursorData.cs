namespace Waggle.Common.Pagination.Cursors
{
    public class CursorData
    {
        public Dictionary<string, object?> Values { get; set; } = new();
        public int Version { get; set; }
    }
}
