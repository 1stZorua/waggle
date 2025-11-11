using Waggle.AuthService.Health;
using Waggle.AuthService.Models;
using Waggle.AuthService.Services;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Waggle.Common.Extensions;
using Waggle.Common.Grpc;
using Waggle.Common.Helpers;
using Waggle.Common.Messaging;
using Waggle.Common.Observability;
using Waggle.Contracts.User.Grpc;
using Waggle.Contracts.User.Interfaces;
using Waggle.AuthService.Grpc;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
    DotNetEnv.Env.Load();

builder.UseCommonSerilog();

builder.Services.Configure<KeycloakSettings>(opt =>
{
    opt.AuthServerUrl = Env.GetRequired("KEYCLOAK_AUTH_SERVER_URL");
    opt.Realm = Env.GetRequired("KEYCLOAK_REALM");
    opt.ClientId = Env.GetRequired("KEYCLOAK_CLIENT_ID");
    opt.ClientSecret = Env.GetRequired("KEYCLOAK_CLIENT_SECRET");
    opt.AdminClientId = Env.GetRequired("KEYCLOAK_ADMIN_CLIENT_ID");
    opt.AdminClientSecret = Env.GetRequired("KEYCLOAK_ADMIN_CLIENT_SECRET");
});

builder.Services.AddCommonAutoMapper();
builder.Services.AddCommonGrpc();
builder.Services.AddCommonApi("Auth Service");
builder.Services.AddCommonObservability("Auth Service");

if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddMessaging(builder.Configuration);

    builder.Services.AddGrpcClient<GrpcUser.GrpcUserClient>(opt =>
    {
        opt.Address = new Uri(builder.Configuration["GrpcUser"]!);
    });
}

builder.Services.AddHttpClient<IKeycloakClient, KeycloakClient>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IKeycloakClient, KeycloakClient>();
builder.Services.AddScoped<IUserDataClient, UserDataClient>();

builder.Services.AddHealthChecks()
    .AddCheck<KeycloakHealthCheck>("keycloak", HealthStatus.Unhealthy);

var app = builder.Build();

app.UseCommonPipeline();
app.UseCommonPrometheusEndpoint();

app.MapCommonHealthChecks();
app.MapCommonGrpcReflection();
app.MapGrpcService<GrpcAuthService>();
app.MapControllers();

app.UseSerilogOnShutdown();

app.Run();

public partial class Program { }
