
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Waggle.Common.Extensions;
using Waggle.Common.Grpc;
using Waggle.Common.Messaging;
using Waggle.Common.Observability;
using Waggle.Contracts.Comment.Grpc;
using Waggle.Contracts.Comment.Interfaces;
using Waggle.Contracts.Post.Grpc;
using Waggle.Contracts.Post.Interfaces;
using Waggle.FavoriteService.Data;
using Waggle.FavoriteService.Grpc;
using Waggle.FavoriteService.Services;
using Waggle.FollowService.Data;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
    DotNetEnv.Env.Load();

builder.UseCommonSerilog();

builder.Services.AddCommonAutoMapper();
builder.Services.AddCommonGrpc();
builder.Services.AddCommonApi("Favorite Service");
builder.Services.AddCommonObservability("Favorite Service");

builder.Services.AddCommonValidation();

if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddMessaging(builder.Configuration, "favorite-service");

    builder.Services.AddGrpcClient<GrpcPost.GrpcPostClient>(opt =>
    {
        opt.Address = new Uri(builder.Configuration["GrpcPost"]!);
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

    builder.Services.AddDbContext<FavoriteDbContext>(opt => opt.UseNpgsql(dataSource));
}

builder.Services.AddScoped<IFavoriteRepository, FavoriteRepository>();
builder.Services.AddScoped<IFavoriteService, FavoriteService>();
builder.Services.AddScoped<IPostDataClient, PostDataClient>();

builder.Services.AddHealthChecks();
builder.Services.AddCommonAuthentication(builder.Environment);

var app = builder.Build();

app.UseCommonPipeline();
app.UseCommonPrometheusEndpoint();

app.MapCommonHealthChecks();
app.MapCommonGrpcReflection();
app.MapGrpcService<GrpcFavoriteService>();
app.MapControllers();

PrepDb.PrepPopulation(app);

app.UseSerilogOnShutdown();

app.Run();

public partial class Program { }
