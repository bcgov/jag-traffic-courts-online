using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Gov.CitizenApi.Features.Disputes;
using Gov.CitizenApi.Features.Disputes.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Gov.CitizenApi.Test.Features.Disputes.Configuration
{
    [ExcludeFromCodeCoverage]
    public class DisputeServiceConfigurationExtensionTest
    {
        [Fact]
#pragma warning disable IDE1006 // Naming Styles
        public void should_register_necessary_service()
#pragma warning restore IDE1006 // Naming Styles
        {
            var services = new ServiceCollection();
            services.AddDisputeService();

            Assert.NotNull(services.Single(x => x.ServiceType == typeof(IDisputeService)));
        }
    }
}
