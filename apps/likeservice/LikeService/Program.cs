
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Waggle.Common.Extensions;
using Waggle.Common.Grpc;
using Waggle.Common.Messaging;
using Waggle.Common.Observability;
using Waggle.Contracts.Comment.Clients;
using Waggle.Contracts.Comment.Grpc;
using Waggle.Contracts.Comment.Interfaces;
using Waggle.Contracts.Post.Clients;
using Waggle.Contracts.Post.Grpc;
using Waggle.Contracts.Post.Interfaces;
using Waggle.LikeService.Consumers;
using Waggle.LikeService.Data;
using Waggle.LikeService.Grpc;
using Waggle.LikeService.Services;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
    DotNetEnv.Env.Load();

builder.UseCommonSerilog();

builder.Services.AddCommonAutoMapper();
builder.Services.AddCommonGrpc();
builder.Services.AddCommonApi("Like Service");
builder.Services.AddCommonObservability("Like Service");

builder.Services.AddCommonValidation();

if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddMessaging(builder.Configuration, "like-service", opt =>
    {
        opt.AddConsumer<CommentDeletedEventConsumer>();
        opt.AddConsumer<PostDeletedEventConsumer>();
        opt.AddConsumer<UserDeletedEventConsumer>();
    });

    builder.Services.AddGrpcClient<GrpcPost.GrpcPostClient>(opt =>
    {
        opt.Address = new Uri(builder.Configuration["GrpcPost"]!);
    });

    builder.Services.AddGrpcClient<GrpcComment.GrpcCommentClient>(opt =>
    {
        opt.Address = new Uri(builder.Configuration["GrpcComment"]!);
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

    builder.Services.AddDbContext<LikeDbContext>(opt => opt.UseNpgsql(dataSource));
}

builder.Services.AddScoped<ILikeRepository, LikeRepository>();
builder.Services.AddScoped<ILikeService, LikeService>();
builder.Services.AddScoped<IPostDataClient, PostDataClient>();
builder.Services.AddScoped<ICommentDataClient, CommentDataClient>();

builder.Services.AddHealthChecks();
builder.Services.AddCommonAuthentication(builder.Environment);

var app = builder.Build();

app.UseCommonPipeline();
app.UseCommonPrometheusEndpoint();

app.MapCommonHealthChecks();
app.MapCommonGrpcReflection();
app.MapGrpcService<GrpcLikeService>();
app.MapControllers();

PrepDb.PrepPopulation(app);

app.UseSerilogOnShutdown();

app.Run();

public partial class Program { }