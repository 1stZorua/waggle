using Microsoft.Extensions.DependencyInjection;

namespace Waggle.Common.Grpc
{
    public static class GrpcExtensions
    {
        public static IServiceCollection AddCommonGrpc(this IServiceCollection services)
        {
            services.AddGrpc();
            services.AddHttpContextAccessor();
            services.AddGrpcReflection();
            return services;
        }
    }
}
