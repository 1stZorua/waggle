using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Waggle.Common.Auth;
using Waggle.Common.Helpers;

namespace Waggle.Common.Extensions
{
    public static class AuthenticationExtensions
    {
        public static IServiceCollection AddCommonAuthentication(this IServiceCollection services, IHostEnvironment env)
        {
            if (env.EnvironmentName == "Testing")
            {
                services.AddAuthentication("Test")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });
                services.AddAuthorization();
                return services;
            }

            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", opt =>
                {
                    opt.Authority = Env.GetRequired("KEYCLOAK_AUTHORITY");
                    opt.Audience = Env.GetRequired("KEYCLOAK_AUDIENCE");
                    opt.RequireHttpsMetadata = true;
                });

            services.AddAuthorization();
            return services;
        }
    }
}