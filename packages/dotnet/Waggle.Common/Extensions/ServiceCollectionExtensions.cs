using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using Waggle.Common.Serialization;

namespace Waggle.Common.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCommonApiConfiguration(
            this IServiceCollection services,
            string apiTitle,
            string apiVersion = "v1")
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
            });

            return services;
        }

        public static IServiceCollection AddCommonAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg => { }, AppDomain.CurrentDomain.GetAssemblies());
            return services;
        }

        public static IServiceCollection AddCommonGrpc(this IServiceCollection services)
        {
            services.AddGrpc();
            services.AddGrpcReflection();
            return services;
        }
    }
}