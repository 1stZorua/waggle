using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using Waggle.AuthService.Models;
using Waggle.Contracts.User.Interfaces;
using WireMock.Server;

namespace Waggle.AuthService.IntegrationTests.Infrastructure
{
    public sealed class CustomWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        public WireMockServer WireMockServer { get; private set; }
        public Mock<IUserDataClient> UserDataClientMock { get; private set; }

        public CustomWebAppFactory()
        {
            WireMockServer = WireMockServer.Start();
            UserDataClientMock = new Mock<IUserDataClient>();
            SetTestEnvironmentVariables();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<IUserDataClient>();
                services.AddSingleton(UserDataClientMock.Object);

                services.PostConfigure<KeycloakSettings>(opt =>
                {
                    opt.AuthServerUrl = WireMockServer.Urls.First();
                });

                services.AddMassTransitTestHarness();
            });
        }

        private void SetTestEnvironmentVariables()
        {
            Environment.SetEnvironmentVariable("KEYCLOAK_AUTH_SERVER_URL", WireMockServer.Urls.First());
            Environment.SetEnvironmentVariable("KEYCLOAK_REALM", "test-realm");
            Environment.SetEnvironmentVariable("KEYCLOAK_CLIENT_ID", "test-client");
            Environment.SetEnvironmentVariable("KEYCLOAK_CLIENT_SECRET", "secret");
            Environment.SetEnvironmentVariable("KEYCLOAK_ADMIN_CLIENT_ID", "admin-client");
            Environment.SetEnvironmentVariable("KEYCLOAK_ADMIN_CLIENT_SECRET", "admin-secret");
        }

        public Task InitializeAsync()
        {
            WireMockServer.Reset();
            UserDataClientMock.Reset();
            return Task.CompletedTask;
        }

        async Task IAsyncLifetime.DisposeAsync()
        {
            WireMockServer?.Stop();
            WireMockServer?.Dispose();
            await DisposeAsync();
        }
    }
}