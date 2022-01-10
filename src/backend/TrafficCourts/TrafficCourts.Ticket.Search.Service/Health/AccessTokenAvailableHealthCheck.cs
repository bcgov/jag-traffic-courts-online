using Microsoft.Extensions.Diagnostics.HealthChecks;
using TrafficCourts.Ticket.Search.Service.Authentication;

namespace TrafficCourts.Ticket.Search.Service.Health
{
    /// <summary>
    /// Represents a "readiness" check. A service that fails a readiness check is considered to be unable to serve traffic temporarily.
    /// The orchestrator doesn't restart a service that fails this check, but stops sending traffic to it until it responds to this check positively again.
    /// </summary>
    public class AccessTokenAvailableHealthCheck : IHealthCheck
    {
        private readonly ITokenCache _tokenCache;
        private readonly ILogger<AccessTokenAvailableHealthCheck> _logger;

        public AccessTokenAvailableHealthCheck(ITokenCache tokenCache, ILogger<AccessTokenAvailableHealthCheck> logger)
        {
            _tokenCache = tokenCache ?? throw new ArgumentNullException(nameof(tokenCache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Checking if access token available");

            HealthCheckResult result;
            if (AccessTokenAvailable)
            {
                result = HealthCheckResult.Healthy();
            }
            else
            {
                using (_logger.BeginScope(new Dictionary<string, object> { { "HealthStatus", context.Registration.FailureStatus } }))
                {
                    _logger.LogInformation("Access token is not available, returning failure status");
                    result = new HealthCheckResult(context.Registration.FailureStatus);
                }
            }

            return Task.FromResult(result);
        }

        private bool AccessTokenAvailable => _tokenCache.GetToken() is not null;
    }
}
