using Microsoft.Extensions.DependencyInjection;

namespace Waggle.Common.Extensions
{
    public static class AutoMapperExtensions
    {
        public static IServiceCollection AddCommonAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg => { }, AppDomain.CurrentDomain.GetAssemblies());
            return services;
        }
    }
}
