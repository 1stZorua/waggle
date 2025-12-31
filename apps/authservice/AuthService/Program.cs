using Microsoft.Extensions.Diagnostics.HealthChecks;
using Waggle.AuthService.Dtos;
using Waggle.AuthService.Grpc;
using Waggle.AuthService.Health;
using Waggle.AuthService.Options;
using Waggle.AuthService.Saga;
using Waggle.AuthService.Saga.Context;
using Waggle.AuthService.Saga.Steps;
using Waggle.AuthService.Services;
using Waggle.Common.Extensions;
using Waggle.Common.Grpc;
using Waggle.Common.Helpers;
using Waggle.Common.Messaging;
using Waggle.Common.Observability;
using Waggle.Common.Saga;
using Waggle.Contracts.User.Clients;
using Waggle.Contracts.User.Grpc;
using Waggle.Contracts.User.Interfaces;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
    DotNetEnv.Env.Load();

builder.UseCommonSerilog();

builder.Services.Configure<KeycloakOptions>(opt =>
{
    opt.AuthServerUrl = Env.GetRequired("KEYCLOAK_AUTH_SERVER_URL");
    opt.Realm = Env.GetRequired("KEYCLOAK_REALM");
    opt.ClientId = Env.GetRequired("KEYCLOAK_CLIENT_ID");
    opt.ClientSecret = Env.GetRequired("KEYCLOAK_CLIENT_SECRET");
    opt.AdminClientId = Env.GetRequired("KEYCLOAK_ADMIN_CLIENT_ID");
    opt.AdminClientSecret = Env.GetRequired("KEYCLOAK_ADMIN_CLIENT_SECRET");
});

builder.Services.Configure<AuthOptions>(opt =>
{
    opt.MinUsernameLength = int.Parse(Env.GetRequired("AUTH_MIN_USERNAME_LENGTH"));
    opt.MaxUsernameLength = int.Parse(Env.GetRequired("AUTH_MAX_USERNAME_LENGTH"));
    opt.MinPasswordLength = int.Parse(Env.GetRequired("AUTH_MIN_PASSWORD_LENGTH"));
    opt.MaxPasswordLength = int.Parse(Env.GetRequired("AUTH_MAX_PASSWORD_LENGTH"));
});

builder.Services.AddCommonAutoMapper();
builder.Services.AddCommonGrpc();
builder.Services.AddCommonApi("Auth Service");
builder.Services.AddCommonObservability("Auth Service");

builder.Services.AddCommonValidation();

if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddMessaging(builder.Configuration, "auth-service");

    builder.Services.AddGrpcClient<GrpcUser.GrpcUserClient>(opt =>
    {
        opt.Address = new Uri(builder.Configuration["GrpcUser"]!);
    });
}

builder.Services.AddHttpClient<IKeycloakClient, KeycloakClient>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IKeycloakClient, KeycloakClient>();
builder.Services.AddScoped<IUserDataClient, UserDataClient>();

builder.Services.AddScoped<CreateUserStep>();
builder.Services.AddScoped<CreateProfileStep>();
builder.Services.AddScoped<NotifyUserRegisteredStep>();

builder.Services.AddScoped<ISagaCoordinator<RegistrationSagaContext, RegisterResponseDto>>(sp =>
{
    var coordinator = new SagaCoordinator<RegistrationSagaContext, RegisterResponseDto>();
    coordinator.AddStep(sp.GetRequiredService<CreateUserStep>());
    coordinator.AddStep(sp.GetRequiredService<CreateProfileStep>());
    coordinator.AddStep(sp.GetRequiredService<NotifyUserRegisteredStep>());
    return coordinator;
});

builder.Services.AddScoped<DeleteAuthUserStep>();
builder.Services.AddScoped<DeleteUserStep>();

builder.Services.AddScoped<ISagaCoordinator<DeletionSagaContext>>(sp =>
{
    var coordinator = new SagaCoordinator<DeletionSagaContext>();
    coordinator.AddStep(sp.GetRequiredService<DeleteAuthUserStep>());
    coordinator.AddStep(sp.GetRequiredService<DeleteUserStep>());
    return coordinator;
});

builder.Services.AddHealthChecks()
    .AddCheck<KeycloakHealthCheck>("keycloak", HealthStatus.Unhealthy);

builder.Services.AddCommonAuthentication(builder.Environment);

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
