using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Waggle.Common.Validation;

namespace Waggle.Common.Extensions
{
    public static class ValidationExtensions
    {
        public static IServiceCollection AddCommonValidation(this IServiceCollection services)
        {
            // Register the service validator
            services.AddScoped<IServiceValidator, ServiceValidator>();

            // Scan the current assembly for all validators, including internal ones
            var assembly = Assembly.GetCallingAssembly();
            services.AddValidatorsFromAssembly(assembly, includeInternalTypes: true);

            return services;
        }
    }
}
