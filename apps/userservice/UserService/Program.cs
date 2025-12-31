using Microsoft.EntityFrameworkCore;
using Waggle.Common.Extensions;
using Waggle.Common.Grpc;
using Waggle.Common.Messaging;
using Waggle.Common.Observability;
using Waggle.Contracts.Follow.Clients;
using Waggle.Contracts.Follow.Grpc;
using Waggle.Contracts.Follow.Interfaces;
using Waggle.Contracts.Media.Clients;
using Waggle.Contracts.Media.Grpc;
using Waggle.Contracts.Media.Interfaces;
using Waggle.Contracts.Post.Clients;
using Waggle.Contracts.Post.Grpc;
using Waggle.Contracts.Post.Interfaces;
using Waggle.UserService.Consumers;
using Waggle.UserService.Data;
using Waggle.UserService.Grpc;
using Waggle.UserService.Services;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
    DotNetEnv.Env.Load();

builder.UseCommonSerilog();

builder.Services.AddCommonAutoMapper();
builder.Services.AddCommonGrpc();
builder.Services.AddCommonApi("User Service");
builder.Services.AddCommonObservability("User Service");

builder.Services.AddCommonValidation();

if (!builder.Environment.IsEnvironment("Testing")) 
{
    builder.Services.AddMessaging(builder.Configuration, "user-service", opt =>
    {
        opt.AddConsumer<RegisteredEventConsumer>();
        opt.AddConsumer<UserDeletedEventConsumer>();
    });

    builder.Services.AddGrpcClient<GrpcMedia.GrpcMediaClient>(opt =>
    {
        opt.Address = new Uri(builder.Configuration["GrpcMedia"]!);
    });

    builder.Services.AddGrpcClient<GrpcPost.GrpcPostClient>(opt =>
    {
        opt.Address = new Uri(builder.Configuration["GrpcPost"]!);
    });

    builder.Services.AddGrpcClient<GrpcFollow.GrpcFollowClient>(opt =>
    {
        opt.Address = new Uri(builder.Configuration["GrpcFollow"]!);
    });

    var connectionString =
        $"Host={Environment.GetEnvironmentVariable("POSTGRES_HOST")};" +
        $"Port={Environment.GetEnvironmentVariable("POSTGRES_PORT")};" +
        $"Database={Environment.GetEnvironmentVariable("POSTGRES_DB")};" +
        $"Username={Environment.GetEnvironmentVariable("POSTGRES_USER")};" +
        $"Password={Environment.GetEnvironmentVariable("POSTGRES_PASSWORD")}";

    builder.Services.AddDbContext<UserDbContext>(opt => opt.UseNpgsql(connectionString));
}

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMediaDataClient, MediaDataClient>();
builder.Services.AddScoped<IPostDataClient, PostDataClient>();
builder.Services.AddScoped<IFollowDataClient, FollowDataClient>();

builder.Services.AddHealthChecks();
builder.Services.AddCommonAuthentication(builder.Environment);

var app = builder.Build();

app.UseCommonPipeline();
app.UseCommonPrometheusEndpoint();

app.MapCommonHealthChecks();
app.MapCommonGrpcReflection();
app.MapGrpcService<GrpcUserService>();
app.MapControllers();

PrepDb.PrepPopulation(app);

app.UseSerilogOnShutdown();

app.Run();

public partial class Program { }
