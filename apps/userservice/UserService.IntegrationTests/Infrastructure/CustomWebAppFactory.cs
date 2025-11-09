using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Testcontainers.PostgreSql;
using Waggle.UserService.Data;

namespace Waggle.UserService.IntegrationTests.Infrastructure
{
    public sealed class CustomWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly PostgreSqlContainer _dbContainer;

        public CustomWebAppFactory()
        {
            _dbContainer = new PostgreSqlBuilder()
                .WithImage("postgres:17")
                .WithDatabase("waggle_users_test")
                .WithUsername("postgres")
                .WithPassword("postgres")
                .WithCleanUp(true)
                .WithAutoRemove(true)
                .Build();
        }

        public string ConnectionString => _dbContainer.GetConnectionString();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                services.RemoveAll<UserDbContext>();
                services.RemoveAll<DbContextOptions<UserDbContext>>();

                services.AddDbContext<UserDbContext>(options =>
                {
                    options.UseNpgsql(ConnectionString);
                    options.EnableSensitiveDataLogging();
                    options.EnableDetailedErrors();
                });

                services.AddMassTransitTestHarness();

                services.AddLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddDebug();
                    logging.SetMinimumLevel(LogLevel.Warning);
                });

                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<UserDbContext>();
                db.Database.EnsureCreated();
            });
        }

        public async Task InitializeAsync() => await _dbContainer.StartAsync();

        async Task IAsyncLifetime.DisposeAsync()
        {
            await _dbContainer.StopAsync();
            await _dbContainer.DisposeAsync();
        }
    }
}