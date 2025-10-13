using AuthService.Models;
using AuthService.Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
    DotNetEnv.Env.Load();

static string GetEnv(string key) => Environment.GetEnvironmentVariable(key)
    ?? throw new InvalidOperationException($"{key} environment variable is not set");

builder.Services.Configure<KeycloakSettings>(opt =>
{
    opt.AuthServerUrl = GetEnv("KEYCLOAK_AUTH_SERVER_URL");
    opt.Realm = GetEnv("KEYCLOAK_REALM");
    opt.ClientId = GetEnv("KEYCLOAK_CLIENT_ID");
    opt.ClientSecret = GetEnv("KEYCLOAK_CLIENT_SECRET");
    opt.AdminClientId = GetEnv("KEYCLOAK_ADMIN_CLIENT_ID");
    opt.AdminClientSecret = GetEnv("KEYCLOAK_ADMIN_CLIENT_SECRET");
});

builder.Services.AddHttpClient<KeycloakService>();

builder.Services.AddScoped<IKeycloakService, KeycloakService>();

builder.Services.AddControllers()
    .AddJsonOptions(o => {
        o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddRouting(opt =>
{
    opt.LowercaseUrls = true;
    opt.LowercaseQueryStrings = true;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
