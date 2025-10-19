using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Services;
using UserService.SyncDataServices.Grpc;
using Waggle.Common.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCommonAutoMapper();
builder.Services.AddCommonGrpc();

builder.Services.AddDbContext<UserDbContext>(opt => opt.UseInMemoryDatabase("Users"));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService.Services.UserService>();

builder.Services.AddCommonApiConfiguration("User Service");

builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseCommonPipeline();

app.MapCommonHealthChecks();
app.MapCommonGrpcReflection();
app.MapGrpcService<GrpcUserService>();
app.MapControllers();

PrepDb.PrepPopulation(app);

app.Run();
