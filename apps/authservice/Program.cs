using AuthService.Health;
using AuthService.Models;
using AuthService.Services;
using AuthService.SyncDataServices.Grpc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using UserService.Grpc;
using Waggle.Common.Extensions;
using Waggle.Common.Helpers;
using Waggle.Contracts.User.Interfaces;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
    DotNetEnv.Env.Load();

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

builder.Services.AddGrpcClient<GrpcUser.GrpcUserClient>(opt =>
{
    opt.Address = new Uri(builder.Configuration["GrpcUser"]!);
});

builder.Services.AddHttpClient<IAuthService, AuthService.Services.AuthService>();
builder.Services.AddScoped<IAuthService, AuthService.Services.AuthService>();
builder.Services.AddScoped<IUserDataClient, UserDataClient>();

builder.Services.AddCommonApiConfiguration("Auth Service");

builder.Services.AddHealthChecks()
    .AddCheck<KeycloakHealthCheck>("keycloak", HealthStatus.Unhealthy);

var app = builder.Build();

app.UseCommonPipeline();

app.MapCommonHealthChecks();
app.MapCommonGrpcReflection();
app.MapGrpcService<GrpcAuthService>();
app.MapControllers();

app.Run();
