using Microsoft.EntityFrameworkCore;
using UserService.Consumers;
using UserService.Data;
using UserService.Services;
using UserService.SyncDataServices.Grpc;
using Waggle.Common.Extensions;
using Waggle.Common.Grpc;
using Waggle.Common.Messaging;
using Waggle.Common.Observability;

var builder = WebApplication.CreateBuilder(args);

builder.UseCommonSerilog();

builder.Services.AddCommonAutoMapper();
builder.Services.AddCommonGrpc();
builder.Services.AddCommonApi("User Service");
builder.Services.AddCommonObservability("User Service");

builder.Services.AddMessaging(builder.Configuration, opt =>
{
    opt.AddConsumer<RegisteredEventConsumer>();
});

builder.Services.AddDbContext<UserDbContext>(opt => opt.UseInMemoryDatabase("Users"));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService.Services.UserService>();

builder.Services.AddHealthChecks();

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
