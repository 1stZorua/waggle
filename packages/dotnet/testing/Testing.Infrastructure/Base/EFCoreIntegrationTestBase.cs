using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Waggle.Common.Models;
using Waggle.Common.Pagination.Models;

namespace Waggle.Testing.Infrastructure.Base
{
    public abstract class EfCoreIntegrationTestBase<TContext> : HttpIntegrationTestBase
        where TContext : DbContext
    {
        protected EfCoreIntegrationTestBase(IServiceProvider services, HttpClient client)
            : base(services, client)
        {
        }

        public override async Task InitializeAsync() => await CleanDatabaseAsync();

        #region Database Helpers

        protected async Task CleanDatabaseAsync()
        {
            using var scope = Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TContext>();
            await CleanDatabaseCoreAsync(context);
            await context.SaveChangesAsync();
        }

        protected abstract Task CleanDatabaseCoreAsync(TContext context);

        protected async Task<TEntity> SeedEntityAsync<TEntity>(TEntity entity)
            where TEntity : class
        {
            using var scope = Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TContext>();
            await context.Set<TEntity>().AddAsync(entity);
            await context.SaveChangesAsync();
            return entity;
        }

        protected async Task<List<TEntity>> SeedEntitiesAsync<TEntity>(IEnumerable<TEntity> entities)
            where TEntity : class
        {
            using var scope = Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TContext>();
            var list = entities.ToList();
            await context.Set<TEntity>().AddRangeAsync(list);
            await context.SaveChangesAsync();
            return list;
        }

        #endregion

        #region Pagination Helpers

        protected Task<ApiResponse<PagedResult<TDto>>> GetPagedAsync<TDto>(string endpoint, PaginationRequest request)
        {
            var query = $"?pageSize={request.PageSize}&direction={request.Direction}";
            if (!string.IsNullOrEmpty(request.Cursor))
                query += $"&cursor={Uri.EscapeDataString(request.Cursor)}";

            return SendRequestAsync<PagedResult<TDto>>(
                new HttpRequestMessage(HttpMethod.Get, GetEndpoint($"{endpoint}{query}")));
        }

        #endregion
    }
}