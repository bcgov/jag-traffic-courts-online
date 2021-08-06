using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Gov.CitizenApi.Health
{
    public class CitizenApiHealthCheck : IHealthCheck
    {

        private readonly ILogger<CitizenApiHealthCheck> _logger;

        public CitizenApiHealthCheck(ILogger<CitizenApiHealthCheck> logger)
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
