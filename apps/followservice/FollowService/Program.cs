
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Waggle.Common.Extensions;
using Waggle.Common.Grpc;
using Waggle.Common.Messaging;
using Waggle.Common.Observability;
using Waggle.Contracts.Post.Grpc;
using Waggle.Contracts.Post.Interfaces;
using Waggle.Contracts.User.Grpc;
using Waggle.Contracts.User.Interfaces;
using Waggle.FollowService.Data;
using Waggle.FollowService.Grpc;
using Waggle.FollowService.Services;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
    DotNetEnv.Env.Load();

builder.UseCommonSerilog();

builder.Services.AddCommonAutoMapper();
builder.Services.AddCommonGrpc();
builder.Services.AddCommonApi("Follow Service");
builder.Services.AddCommonObservability("Follow Service");

builder.Services.AddCommonValidation();

if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddMessaging(builder.Configuration, "follow-service");

    builder.Services.AddGrpcClient<GrpcPost.GrpcPostClient>(opt =>
    {
        opt.Address = new Uri(builder.Configuration["GrpcPost"]!);
    });

    builder.Services.AddGrpcClient<GrpcUser.GrpcUserClient>(opt =>
    {
        opt.Address = new Uri(builder.Configuration["GrpcUser"]!);
    });

    var connectionString =
        $"Host={Environment.GetEnvironmentVariable("POSTGRES_HOST")};" +
        $"Port={Environment.GetEnvironmentVariable("POSTGRES_PORT")};" +
        $"Database={Environment.GetEnvironmentVariable("POSTGRES_DB")};" +
        $"Username={Environment.GetEnvironmentVariable("POSTGRES_USER")};" +
        $"Password={Environment.GetEnvironmentVariable("POSTGRES_PASSWORD")}";

    var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
    dataSourceBuilder.EnableDynamicJson();

    var dataSource = dataSourceBuilder.Build();

    builder.Services.AddDbContext<FollowDbContext>(opt => opt.UseNpgsql(dataSource));
}

builder.Services.AddScoped<IFollowRepository, FollowRepository>();
builder.Services.AddScoped<IFollowService, FollowService>();
builder.Services.AddScoped<IPostDataClient, PostDataClient>();
builder.Services.AddScoped<IUserDataClient, UserDataClient>();

builder.Services.AddHealthChecks();
builder.Services.AddCommonAuthentication(builder.Environment);

var app = builder.Build();

app.UseCommonPipeline();
app.UseCommonPrometheusEndpoint();

app.MapCommonHealthChecks();
app.MapCommonGrpcReflection();
app.MapGrpcService<GrpcFollowService>();
app.MapControllers();

PrepDb.PrepPopulation(app);

app.UseSerilogOnShutdown();

app.Run();

public partial class Program { }
