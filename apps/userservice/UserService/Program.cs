using Microsoft.EntityFrameworkCore;
using Waggle.UserService.Consumers;
using Waggle.Common.Extensions;
using Waggle.Common.Grpc;
using Waggle.Common.Messaging;
using Waggle.Common.Observability;
using Waggle.UserService.Data;
using Waggle.UserService.Services;
using Waggle.UserService.Grpc;

var builder = WebApplication.CreateBuilder(args);

builder.UseCommonSerilog();

builder.Services.AddCommonAutoMapper();
builder.Services.AddCommonGrpc();
builder.Services.AddCommonApi("User Service");
builder.Services.AddCommonObservability("User Service");

if (!builder.Environment.IsEnvironment("Testing")) 
{
    builder.Services.AddMessaging(builder.Configuration, opt =>
    {
        opt.AddConsumer<RegisteredEventConsumer>();
        opt.AddConsumer<DeletedEventConsumer>();
    });

    builder.Services.AddDbContext<UserDbContext>(opt => opt.UseInMemoryDatabase("Users"));
}
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

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

public partial class Program { }
