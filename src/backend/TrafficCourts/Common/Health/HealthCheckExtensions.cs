using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

namespace TrafficCourts.Common.Health;

public static class HealthCheckExtensions
{
    public static IHostApplicationBuilder AddDefaultHealthChecks(this IHostApplicationBuilder builder)
    {
        builder.Services.AddHealthChecks()
            // Add a default liveness check to ensure app is responsive
            .AddCheck("self", () => HealthCheckResult.Healthy(), [HealthCheckType.Liveness]);

        return builder;
    }

    /// <summary>
    /// Adds the &quot;/health/live&quot;, &quot;/health/ready&quot; and &quot;/health/status&quot
    /// endpoints.
    /// </summary>
    public static WebApplication MapDefaultHealthCheckEndpoints(this WebApplication app)
    {
        // Only health checks tagged with "liveness" tag must pass for the app to be considered live
        app.MapHealthChecks($"/health/live", new HealthCheckOptions()
        {
            Predicate = (check) => check.Tags.Contains(HealthCheckType.Liveness)
        });

        // Only health checks tagged with "readiness" tag must pass for the app to be considered ready
        app.MapHealthChecks($"/health/ready", new HealthCheckOptions()
        {
            Predicate = (check) => check.Tags.Contains(HealthCheckType.Readiness)
        });

        // all the health checks
        app.MapHealthChecks($"/health/status");

        return app;
    }
}
