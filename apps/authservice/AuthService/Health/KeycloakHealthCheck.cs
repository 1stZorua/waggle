using Waggle.AuthService.Constants;
using Waggle.AuthService.Logging;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Waggle.Common.Helpers;

namespace Waggle.AuthService.Health
{
    internal sealed class KeycloakHealthCheck : IHealthCheck
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<KeycloakHealthCheck> _logger;

        public KeycloakHealthCheck(HttpClient httpClient, ILogger<KeycloakHealthCheck> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            var keycloakUrl = Env.GetRequired("KEYCLOAK_AUTH_SERVER_URL");
            var realm = Env.GetRequired("KEYCLOAK_REALM");

            var healthUrl = $"{keycloakUrl.TrimEnd('/')}/realms/{realm}";

            try
            {
                var response = await _httpClient.GetAsync(healthUrl, cancellationToken);

                if (response.IsSuccessStatusCode)
                    return HealthCheckResult.Healthy();

                _logger.LogKeycloakHealthCheckFailed(response.StatusCode, healthUrl);
                return HealthCheckResult.Unhealthy(AuthErrors.Service.Unavailable);
            }
            catch (Exception ex)
            {
                _logger.LogKeycloakHealthCheckException(ex, healthUrl);
                return HealthCheckResult.Unhealthy(AuthErrors.Service.Unavailable);
            }
        }
    }
}
