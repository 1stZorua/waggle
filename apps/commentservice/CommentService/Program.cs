
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Waggle.CommentService.Consumers;
using Waggle.CommentService.Data;
using Waggle.CommentService.Grpc;
using Waggle.CommentService.Saga.Context;
using Waggle.CommentService.Saga.Steps;
using Waggle.CommentService.Services;
using Waggle.Common.Extensions;
using Waggle.Common.Grpc;
using Waggle.Common.Messaging;
using Waggle.Common.Observability;
using Waggle.Common.Saga;
using Waggle.Contracts.Like.Clients;
using Waggle.Contracts.Like.Grpc;
using Waggle.Contracts.Like.Interfaces;
using Waggle.Contracts.Post.Clients;
using Waggle.Contracts.Post.Grpc;
using Waggle.Contracts.Post.Interfaces;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
    DotNetEnv.Env.Load();

builder.UseCommonSerilog();

builder.Services.AddCommonAutoMapper();
builder.Services.AddCommonGrpc();
builder.Services.AddCommonApi("Comment Service");
builder.Services.AddCommonObservability("Comment Service");

builder.Services.AddCommonValidation();

if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddMessaging(builder.Configuration, "comment-service", opt =>
    {
        opt.AddConsumer<PostDeletedEventConsumer>();
        opt.AddConsumer<UserDeletedEventConsumer>();
    });

    builder.Services.AddGrpcClient<GrpcPost.GrpcPostClient>(opt =>
    {
        opt.Address = new Uri(builder.Configuration["GrpcPost"]!);
    });

    builder.Services.AddGrpcClient<GrpcLike.GrpcLikeClient>(opt =>
    {
        opt.Address = new Uri(builder.Configuration["GrpcLike"]!);
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

    builder.Services.AddDbContext<CommentDbContext>(opt => opt.UseNpgsql(dataSource));
}

builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IPostDataClient, PostDataClient>();
builder.Services.AddScoped<ILikeDataClient, LikeDataClient>();

builder.Services.AddScoped<DeleteCommentStep>();
builder.Services.AddScoped<CleanupStep>();

builder.Services.AddScoped<ISagaCoordinator<DeletionSagaContext>>(sp =>
{
    var coordinator = new SagaCoordinator<DeletionSagaContext>();
    coordinator.AddStep(sp.GetRequiredService<DeleteCommentStep>());
    coordinator.AddStep(sp.GetRequiredService<CleanupStep>());
    return coordinator;
});

builder.Services.AddHealthChecks();
builder.Services.AddCommonAuthentication(builder.Environment);

var app = builder.Build();

app.UseCommonPipeline();
app.UseCommonPrometheusEndpoint();

app.MapCommonHealthChecks();
app.MapCommonGrpcReflection();
app.MapGrpcService<GrpcCommentService>();
app.MapControllers();

PrepDb.PrepPopulation(app);

app.UseSerilogOnShutdown();

app.Run();

public partial class Program { }