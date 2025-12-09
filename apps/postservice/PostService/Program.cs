
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Waggle.Common.Extensions;
using Waggle.Common.Grpc;
using Waggle.Common.Messaging;
using Waggle.Common.Observability;
using Waggle.Contracts.Media.Grpc;
using Waggle.Contracts.Media.Interfaces;
using Waggle.PostService.Data;
using Waggle.PostService.Grpc;
using Waggle.PostService.Services;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
    DotNetEnv.Env.Load();

builder.UseCommonSerilog();

builder.Services.AddCommonAutoMapper();
builder.Services.AddCommonGrpc();
builder.Services.AddCommonApi("Post Service");
builder.Services.AddCommonObservability("Post Service");

builder.Services.AddCommonValidation();

if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddMessaging(builder.Configuration);

    builder.Services.AddGrpcClient<GrpcMedia.GrpcMediaClient>(opt =>
    {
        opt.Address = new Uri(builder.Configuration["GrpcMedia"]!);
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

    builder.Services.AddDbContext<PostDbContext>(opt => opt.UseNpgsql(dataSource));
}

builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<IMediaDataClient, MediaDataClient>();

builder.Services.AddHealthChecks();
builder.Services.AddCommonAuthentication(builder.Environment);

var app = builder.Build();

app.UseCommonPipeline();
app.UseCommonPrometheusEndpoint();

app.MapCommonHealthChecks();
app.MapCommonGrpcReflection();
app.MapGrpcService<GrpcPostService>();
app.MapControllers();

app.UseSerilogOnShutdown();

app.Run();

public partial class Program { }
