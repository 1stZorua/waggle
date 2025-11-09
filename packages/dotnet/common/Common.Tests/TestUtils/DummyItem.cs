namespace Waggle.Common.Tests.TestUtils
{
    public static class DummyItem
    {
        public static TestItem Create(
            int? id = null,
            string? name = null)
        {
            return new TestItem
            {
                Id = id ?? 1,
                Name = name ?? $"Item {id ?? 1}"
            };
        }

        public static List<TestItem> CreateList(int count)
        {
            return [.. Enumerable.Range(1, count).Select(i => Create(i))];
        }
    }

    public class TestItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
