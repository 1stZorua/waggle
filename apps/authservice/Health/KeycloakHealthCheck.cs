using Microsoft.Extensions.Diagnostics.HealthChecks;
using Waggle.Common.Helpers;

namespace AuthService.Health
{
    internal sealed class KeycloakHealthCheck : IHealthCheck
    {
        private readonly HttpClient _httpClient;

        public KeycloakHealthCheck(HttpClient httpClient)
        {
            _httpClient = httpClient;
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

                return response.IsSuccessStatusCode 
                    ? HealthCheckResult.Healthy()
                    : HealthCheckResult.Unhealthy($"Keycloak returned status {response.StatusCode}.");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy($"Keycloak check failed: {ex.Message}");
            }
        }
    }
}
