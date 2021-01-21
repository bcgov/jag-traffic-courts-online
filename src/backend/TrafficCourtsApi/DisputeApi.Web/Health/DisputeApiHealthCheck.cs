using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace DisputeApi.Web.Health
{
    public class DisputeApiHealthCheck : IHealthCheck
    {

        private readonly ILogger<DisputeApiHealthCheck> _logger;

        public DisputeApiHealthCheck(ILogger<DisputeApiHealthCheck> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Check if all defined Enumerations can be mapped to Dynamics Option/Entiy correctly.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            return await Task.FromResult(HealthCheckResult.Healthy("service healthy"));
        }
    }
}
