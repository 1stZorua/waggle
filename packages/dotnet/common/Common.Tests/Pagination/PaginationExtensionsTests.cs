using System.Linq.Expressions;
using Waggle.Common.Pagination.Core;
using Waggle.Common.Pagination.Models;

namespace Waggle.Common.Tests.Pagination
{
    public class PaginationExtensionsTests
    {
        [Theory]
        [InlineData(PaginationDirection.Forward, false)]
        [InlineData(PaginationDirection.Backward, true)]
        public async Task ToPagedAsync_BasicPagination_ReturnsCorrectItems(PaginationDirection direction, bool shouldReverse)
        {
            // Arrange
            var items = DummyItem.CreateList(20);
            var queryable = items.AsQueryable();

            var sortFields = new (Expression<Func<TestItem, object>> SortBy, string Name)[]
            {
                (x => x.Id, nameof(TestItem.Id))
            };

            var request = new PaginationRequest
            {
                PageSize = 5,
                Direction = direction
            };

            // Act
            var result = await queryable.ToPagedAsync(sortFields, request);

            // Assert
            result.Items.Count.Should().Be(5);
            result.Items.First().Id.Should().Be(shouldReverse ? 16 : 1);
            result.Items.Last().Id.Should().Be(shouldReverse ? 20 : 5);
            result.PageInfo.HasNextPage.Should().Be(!shouldReverse);
            result.PageInfo.HasPreviousPage.Should().Be(shouldReverse);
        }

        [Fact]
        public async Task ToPagedAsync_WithCursor_FiltersCorrectly()
        {
            // Arrange
            var items = DummyItem.CreateList(10);
            var queryable = items.AsQueryable();

            var sortFields = new (Expression<Func<TestItem, object>> SortBy, string Name)[]
            {
                (x => x.Id, nameof(TestItem.Id))
            };

            var cursorValues = new Dictionary<string, object?> { { nameof(TestItem.Id), 5 } };

            var cursorValue = cursorValues[nameof(TestItem.Id)] as int? ?? 0;
            var filteredQuery = queryable.Where(x => x.Id > cursorValue);

            var request = new PaginationRequest
            {
                PageSize = 3,
                Cursor = null
            };

            // Act
            var result = await filteredQuery.ToPagedAsync(sortFields, request);

            // Assert
            result.Items.Select(x => x.Id).Should().Equal(6, 7, 8);
        }

        [Fact]
        public async Task ToPagedAsync_EmptyList_ReturnsEmptyPage()
        {
            // Arrange
            var queryable = new List<TestItem>().AsQueryable();
            var sortFields = new (Expression<Func<TestItem, object>> SortBy, string Name)[]
            {
                (x => x.Id, nameof(TestItem.Id))
            };
            var request = new PaginationRequest { PageSize = 5 };

            // Act
            var result = await queryable.ToPagedAsync(sortFields, request);

            // Assert
            result.Items.Should().BeEmpty();
            result.PageInfo.HasNextPage.Should().BeFalse();
            result.PageInfo.HasPreviousPage.Should().BeFalse();
        }

        [Fact]
        public async Task ToPagedAsync_PageSizeGreaterThanItemCount_ReturnsAllItems()
        {
            // Arrange
            var items = DummyItem.CreateList(3);
            var queryable = items.AsQueryable();
            var sortFields = new (Expression<Func<TestItem, object>> SortBy, string Name)[]
            {
                (x => x.Id, nameof(TestItem.Id))
            };
            var request = new PaginationRequest { PageSize = 10 };

            // Act
            var result = await queryable.ToPagedAsync(sortFields, request);

            // Assert
            result.Items.Count.Should().Be(3);
            result.PageInfo.HasNextPage.Should().BeFalse();
        }

        [Fact]
        public async Task ToPagedAsync_PageSizeZero_UsesDefaultPageSize()
        {
            // Arrange
            var items = DummyItem.CreateList(50);
            var queryable = items.AsQueryable();
            var sortFields = new (Expression<Func<TestItem, object>> SortBy, string Name)[]
            {
                (x => x.Id, nameof(TestItem.Id))
            };
            var request = new PaginationRequest { PageSize = 0 };

            // Act
            var result = await queryable.ToPagedAsync(sortFields, request);

            // Assert
            result.Items.Count.Should().Be(20);
            result.PageInfo.PageSize.Should().Be(20);
        }

        [Fact]
        public async Task ToPagedAsync_MultiFieldSort_ReturnsCorrectOrder()
        {
            // Arrange
            var items = new List<TestItem>
            {
                DummyItem.Create(1, "B"),
                DummyItem.Create(2, "A"),
                DummyItem.Create(3, "A"),
                DummyItem.Create(4, "B")
            };
            var queryable = items.AsQueryable();

            var sortFields = new (Expression<Func<TestItem, object>> SortBy, string Name)[]
            {
                (x => x.Name, nameof(TestItem.Name)),
                (x => x.Id, nameof(TestItem.Id))
            };

            var request = new PaginationRequest { PageSize = 4 };

            // Act
            var result = await queryable.ToPagedAsync(sortFields, request);

            // Assert
            result.Items.Select(x => x.Id).Should().Equal(2, 3, 1, 4);
        }
    }
}
