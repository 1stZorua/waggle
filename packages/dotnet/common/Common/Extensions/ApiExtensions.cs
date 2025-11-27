using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using Waggle.Common.Serialization;

namespace Waggle.Common.Extensions
{
    public static class ApiExtensions
    {
        public static IServiceCollection AddCommonApi(this IServiceCollection services, string apiTitle, string apiVersion = "v1")
        {
            services.AddControllers()
                .AddJsonOptions(o =>
                {
                    o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                    o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(new LowerCaseNamingPolicy()));
                });

            services.AddRouting(opt =>
            {
                opt.LowercaseUrls = true;
                opt.LowercaseQueryStrings = true;
            });

            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(apiVersion, new OpenApiInfo { Title = apiTitle, Version = apiVersion });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = "Enter: Bearer {token}"
                });

                c.AddSecurityRequirement(new()
                {
                    {
                        new() { Reference = new() { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
                        Array.Empty<string>()
                    }
                });
            });

            return services;
        }
    }
}
