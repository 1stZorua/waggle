using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using Waggle.AuthService.Options;
using Waggle.Common.Messaging;
using Waggle.Contracts.User.Interfaces;
using WireMock.Server;

namespace Waggle.AuthService.IntegrationTests.Infrastructure
{
    public sealed class CustomWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        public WireMockServer WireMockServer { get; private set; }
        public Mock<IUserDataClient> UserDataClientMock { get; private set; }
        public Mock<IEventPublisher> EventPublisherMock { get; private set; }

        public CustomWebAppFactory()
        {
            WireMockServer = WireMockServer.Start();
            UserDataClientMock = new Mock<IUserDataClient>();
            EventPublisherMock = new Mock<IEventPublisher>();
            SetTestEnvironmentVariables();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<IUserDataClient>();
                services.AddSingleton(UserDataClientMock.Object);

                services.PostConfigure<KeycloakOptions>(opt =>
                {
                    opt.AuthServerUrl = WireMockServer.Urls.First();
                });

                services.AddMassTransitTestHarness(cfg =>
                {
                    cfg.UsingInMemory((context, bus) =>
                    {
                        bus.ConfigureEndpoints(context);
                    });
                });

                services.RemoveAll<IEventPublisher>();
                services.AddScoped<IEventPublisher>(sp =>
                {
                    var publishEndpoint = sp.GetRequiredService<IPublishEndpoint>();
                    return new MassTransitEventPublisher(publishEndpoint);
                });
            });
        }

        private void SetTestEnvironmentVariables()
        {
            var authServerUrl = WireMockServer.Urls.First();
            var realm = "test-realm";
            var clientId = "test-client";

            Environment.SetEnvironmentVariable("KEYCLOAK_AUTH_SERVER_URL", authServerUrl);
            Environment.SetEnvironmentVariable("KEYCLOAK_REALM", realm);
            Environment.SetEnvironmentVariable("KEYCLOAK_CLIENT_ID", clientId);
            Environment.SetEnvironmentVariable("KEYCLOAK_CLIENT_SECRET", "secret");
            Environment.SetEnvironmentVariable("KEYCLOAK_ADMIN_CLIENT_ID", "admin-client");
            Environment.SetEnvironmentVariable("KEYCLOAK_ADMIN_CLIENT_SECRET", "admin-secret");

            Environment.SetEnvironmentVariable("KEYCLOAK_AUTHORITY", $"{authServerUrl}/realms/{realm}");
            Environment.SetEnvironmentVariable("KEYCLOAK_AUDIENCE", clientId);

            Environment.SetEnvironmentVariable("AUTH_MIN_USERNAME_LENGTH", "3");
            Environment.SetEnvironmentVariable("AUTH_MAX_USERNAME_LENGTH", "20");
            Environment.SetEnvironmentVariable("AUTH_MIN_PASSWORD_LENGTH", "4");
            Environment.SetEnvironmentVariable("AUTH_MAX_PASSWORD_LENGTH", "128");
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