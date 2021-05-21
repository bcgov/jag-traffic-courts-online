using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DisputeApi.Web.Features.Disputes;
using DisputeApi.Web.Features.Disputes.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DisputeApi.Web.Test.Features.Disputes.Configuration
{
    [ExcludeFromCodeCoverage]
    public class DisputeServiceConfigurationExtensionTest
    {
        [Fact]
        public void should_register_necessary_service()
        {
            var services = new ServiceCollection();
            services.AddDisputeService();

            Assert.NotNull(services.Single(x => x.ServiceType == typeof(IDisputeService)));
        }
    }
}
