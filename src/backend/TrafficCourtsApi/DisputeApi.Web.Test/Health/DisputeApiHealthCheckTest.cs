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
#pragma warning disable IDE1006 // Naming Styles
        public async Task return_health_service_for_dispute()
#pragma warning restore IDE1006 // Naming Styles
        {
            var result = await _sut.CheckHealthAsync(new HealthCheckContext(), CancellationToken.None);
            Assert.Equal(HealthStatus.Healthy, result.Status);
        }
    }
}
