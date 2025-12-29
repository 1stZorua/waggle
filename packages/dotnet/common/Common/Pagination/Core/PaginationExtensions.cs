using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using Waggle.Common.Pagination.Cursors;
using Waggle.Common.Pagination.Models;

namespace Waggle.Common.Pagination.Core
{
    public static class PaginationExtensions
    {
        private static readonly ConcurrentDictionary<Expression, Delegate> _compiledCache = new();

        /// <summary>
        /// Executes a query with cursor-based pagination and returns a paged result.
        /// </summary>
        public static async Task<PagedResult<T>> ToPagedAsync<T>(
            this IQueryable<T> query,
            (Expression<Func<T, object>> SortBy, string Name)[] sortFields,
            PaginationRequest request,
            CancellationToken ct = default)
        {
            request.Validate();
            var isBackward = request.Direction == PaginationDirection.Backward;
            var cursorValues = CursorHelper.Decode(request.Cursor);
            var hasCursor = cursorValues != null && cursorValues.Count > 0;
            var isDescending = true;

            if (cursorValues != null)
                query = ApplyCursorFilter(query, sortFields, cursorValues, isBackward, isDescending);

            query = ApplyOrder(query, sortFields, isDescending);

            List<T> items;
            if (query.Provider is IAsyncQueryProvider)
                items = await query.Take(request.PageSize + 1).ToListAsync(ct);
            else
                items = [.. query.Take(request.PageSize + 1)];

            var hasMore = items.Count > request.PageSize;
            if (hasMore) items = items.Take(request.PageSize).ToList();
            if (isBackward) items.Reverse();

            string? nextCursor = null;
            string? previousCursor = null;

            var hasNextPage = !isBackward && hasMore;

            var hasPreviousPage = !isBackward && hasCursor;

            if (items.Count > 0)
            {
                if (hasNextPage)
                    nextCursor = MakeCursor(items.Last(), sortFields);

                if (hasPreviousPage)
                    previousCursor = MakeCursor(items.First(), sortFields);
            }

            return new PagedResult<T>
            {
                Items = items,
                PageInfo = new PageInfo
                {
                    PageSize = request.PageSize,
                    NextCursor = nextCursor,
                    PreviousCursor = previousCursor,
                    HasNextPage = hasNextPage,
                    HasPreviousPage = hasPreviousPage
                }
            };
        }

        /// <summary>
        /// Applies filtering based on cursor values for multi-field sorting.
        /// </summary>
        private static IQueryable<T> ApplyCursorFilter<T>(
            IQueryable<T> query,
            (Expression<Func<T, object>> SortBy, string Name)[] sortFields,
            Dictionary<string, object?> cursorValues,
            bool backward,
            bool descending)
        {
            var param = Expression.Parameter(typeof(T), "x");
            Expression? predicate = null;

            for (int i = 0; i < sortFields.Length; i++)
            {
                var (sortBy, name) = sortFields[i];
                if (!cursorValues.TryGetValue(name, out var value) || value == null) continue;

                var fieldExpr = Expression.Convert(Expression.Invoke(sortBy, param), value.GetType());

                Expression comparison;
                if (descending)
                {
                    comparison = backward
                        ? Expression.GreaterThan(fieldExpr, Expression.Constant(value))
                        : Expression.LessThan(fieldExpr, Expression.Constant(value));
                }
                else
                {
                    comparison = backward
                        ? Expression.LessThan(fieldExpr, Expression.Constant(value))
                        : Expression.GreaterThan(fieldExpr, Expression.Constant(value));
                }

                if (i > 0)
                {
                    var equalityChain = Enumerable.Range(0, i)
                        .Select(j =>
                        {
                            var (prevSortBy, prevName) = sortFields[j];
                            if (!cursorValues.TryGetValue(prevName, out var prevValue) || prevValue == null)
                                return null;
                            var prevExpr = Expression.Convert(Expression.Invoke(prevSortBy, param), prevValue.GetType());
                            return Expression.Equal(prevExpr, Expression.Constant(prevValue));
                        })
                        .Where(e => e != null)
                        .Select(e => e!)
                        .Aggregate((left, right) => Expression.AndAlso(left, right));

                    comparison = Expression.AndAlso(equalityChain, comparison);
                }

                predicate = predicate == null ? comparison : Expression.OrElse(predicate, comparison);
            }

            return predicate != null ? query.Where(Expression.Lambda<Func<T, bool>>(predicate, param)) : query;
        }

        /// <summary>
        /// Applies ordering based on multiple fields with ascending/descending support.
        /// </summary>
        private static IQueryable<T> ApplyOrder<T>(
            IQueryable<T> query,
            (Expression<Func<T, object>> SortBy, string Name)[] sortFields,
            bool descending)
        {
            IOrderedQueryable<T>? ordered = null;

            for (int i = 0; i < sortFields.Length; i++)
            {
                var sortBy = sortFields[i].SortBy;
                ordered = i == 0
                    ? descending ? query.OrderByDescending(sortBy) : query.OrderBy(sortBy)
                    : descending ? ordered!.ThenByDescending(sortBy) : ordered!.ThenBy(sortBy);
            }

            return ordered ?? query;
        }

        /// <summary>
        /// Creates a cursor string for a single item using the specified sort fields.
        /// </summary>
        private static string MakeCursor<T>(T item, (Expression<Func<T, object>> SortBy, string Name)[] sortFields)
        {
            var values = sortFields.ToDictionary(
                sf => sf.Name,
                sf => GetCompiled(sf.SortBy)(item)
            );
            return CursorHelper.Encode(values!);
        }

        /// <summary>
        /// Compiles and caches expressions for performance.
        /// </summary>
        private static Func<T, object> GetCompiled<T>(Expression<Func<T, object>> expr)
        {
            return (Func<T, object>)_compiledCache.GetOrAdd(expr, e => expr.Compile());
        }
    }
}