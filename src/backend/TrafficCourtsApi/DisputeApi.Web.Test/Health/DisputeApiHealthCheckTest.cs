using System.Diagnostics.CodeAnalysis;
using DisputeApi.Web.Health;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DisputeApi.Web.Test.Health
{
    [ExcludeFromCodeCoverage]
    public class DisputeApiHealthCheckTest
    {
        private DisputeApiHealthCheck _sut;
        private readonly Mock<ILogger<DisputeApiHealthCheck>> _apiServiceLogger = new Mock<ILogger<DisputeApiHealthCheck>>();

        public DisputeApiHealthCheckTest()
        {
            _sut = new DisputeApiHealthCheck(_apiServiceLogger.Object);
        }

        [Fact]
        public async Task return_health_service_for_dispute()
        {
            var result = await _sut.CheckHealthAsync(new HealthCheckContext(), CancellationToken.None);
            Assert.Equal(HealthStatus.Healthy, result.Status);
        }
    }
}
