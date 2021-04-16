using System.Diagnostics.CodeAnalysis;
using DisputeApi.Web.Health;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;

namespace DisputeApi.Web.Test.Health
{
    [ExcludeFromCodeCoverage]
    public class DisputeApiHealthCheckTest
    {
        private DisputeApiHealthCheck _sut;
        private readonly Mock<ILogger<DisputeApiHealthCheck>> _apiServiceLogger = new Mock<ILogger<DisputeApiHealthCheck>>();


        [SetUp]
        public void SetUp()
        {
            _sut = new DisputeApiHealthCheck(_apiServiceLogger.Object);
        }

        [Test]
        public async Task return_health_service_for_dispute()
        {
            var result = await _sut.CheckHealthAsync(new HealthCheckContext(), CancellationToken.None);
            Assert.AreEqual(HealthStatus.Healthy, result.Status);
        }
    }
}
