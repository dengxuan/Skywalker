using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Skywalker.HealthChecks;
using Skywalker.HealthChecks.AspNetCore;

namespace Microsoft.AspNetCore.Builder;

/// <summary>
/// Extension methods for mapping health check endpoints.
/// </summary>
public static class HealthCheckApplicationBuilderExtensions
{
    /// <summary>
    /// Maps Skywalker health check endpoints.
    /// </summary>
    /// <param name="endpoints">The endpoint route builder.</param>
    /// <returns>The endpoint route builder.</returns>
    public static IEndpointRouteBuilder MapSkywalkerHealthChecks(this IEndpointRouteBuilder endpoints)
    {
        var options = endpoints.ServiceProvider.GetService<SkywalkerHealthCheckOptions>()
            ?? new SkywalkerHealthCheckOptions();

        // Simple health endpoint
        endpoints.MapHealthChecks(options.HealthEndpoint, new HealthCheckOptions
        {
            ResponseWriter = HealthCheckResponseWriter.WriteSimpleResponse
        });

        // Detailed health endpoint
        if (options.EnableDetailedEndpoint)
        {
            var detailedBuilder = endpoints.MapHealthChecks(options.DetailedEndpoint, new HealthCheckOptions
            {
                ResponseWriter = HealthCheckResponseWriter.WriteDetailedResponse
            });

            if (options.RequireAuthorizationForDetailedEndpoint)
            {
                if (!string.IsNullOrEmpty(options.DetailedEndpointAuthorizationPolicy))
                {
                    detailedBuilder.RequireAuthorization(options.DetailedEndpointAuthorizationPolicy);
                }
                else
                {
                    detailedBuilder.RequireAuthorization();
                }
            }
        }

        // Kubernetes-style endpoints
        if (options.EnableKubernetesEndpoints)
        {
            // Ready endpoint - checks all services tagged with "ready"
            endpoints.MapHealthChecks(options.ReadyEndpoint, new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains(HealthCheckConsts.ReadyTag),
                ResponseWriter = HealthCheckResponseWriter.WriteSimpleResponse
            });

            // Live endpoint - basic liveness check
            endpoints.MapHealthChecks(options.LiveEndpoint, new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains(HealthCheckConsts.LiveTag),
                ResponseWriter = HealthCheckResponseWriter.WriteSimpleResponse
            });
        }

        return endpoints;
    }
}

