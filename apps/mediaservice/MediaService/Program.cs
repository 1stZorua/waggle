using Microsoft.EntityFrameworkCore;
using Minio;
using Waggle.Common.Extensions;
using Waggle.Common.Grpc;
using Waggle.Common.Messaging;
using Waggle.Common.Observability;
using Waggle.MediaService.Consumers;
using Waggle.MediaService.Data;
using Waggle.MediaService.Grpc;
using Waggle.MediaService.Infrastructure;
using Waggle.MediaService.Services;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
    DotNetEnv.Env.Load();

builder.UseCommonSerilog();

builder.Services.AddCommonAutoMapper();
builder.Services.AddCommonGrpc();
builder.Services.AddCommonApi("Media Service");
builder.Services.AddCommonObservability("Media Service");

builder.Services.AddCommonValidation();

if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddMessaging(builder.Configuration, "media-service", opt =>
    {
        opt.AddConsumer<PostDeletedEventConsumer>();
        opt.AddConsumer<UserDeletedEventConsumer>();
    });

    var connectionString =
        $"Host={Environment.GetEnvironmentVariable("POSTGRES_HOST")};" +
        $"Port={Environment.GetEnvironmentVariable("POSTGRES_PORT")};" +
        $"Database={Environment.GetEnvironmentVariable("POSTGRES_DB")};" +
        $"Username={Environment.GetEnvironmentVariable("POSTGRES_USER")};" +
        $"Password={Environment.GetEnvironmentVariable("POSTGRES_PASSWORD")}";

    builder.Services.AddDbContext<MediaDbContext>(opt => opt.UseNpgsql(connectionString));

    var endpoint = Environment.GetEnvironmentVariable("MINIO_ENDPOINT");
    var user = Environment.GetEnvironmentVariable("MINIO_ROOT_USER");
    var password = Environment.GetEnvironmentVariable("MINIO_ROOT_PASSWORD");

    builder.Services.AddMinio(c => c
        .WithEndpoint(endpoint)
        .WithCredentials(user, password)
        .WithSSL(false)
        .Build()
    );
}

builder.Services.AddScoped<IMediaRepository, MediaRepository>();
builder.Services.AddScoped<IMediaStorageService, MinioStorageService>();
builder.Services.AddScoped<IMediaService, MediaService>();

builder.Services.AddHealthChecks();
builder.Services.AddCommonAuthentication(builder.Environment);

var app = builder.Build();

if (!app.Environment.IsEnvironment("Testing"))
    await MinioInitializer.EnsureBucketsExistAsync(app.Services);

app.UseCommonPipeline();
app.UseCommonPrometheusEndpoint();

app.MapCommonHealthChecks();
app.MapCommonGrpcReflection();
app.MapGrpcService<GrpcMediaService>();
app.MapControllers();

PrepDb.PrepPopulation(app);

app.UseSerilogOnShutdown();

app.Run();

public partial class Program { }

