using DisputeApi.Web.Features.TcoDispute.Configuration;
using DisputeApi.Web.Features.TcoDispute.Service;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Linq;

namespace DisputeApi.Web.Test.Features.TcoDispute.Configuration
{
    public class DisputeServiceConfigurationExtensionTest
    {
        [Test]
        public void should_register_necessary_service()
        {
            var services = new ServiceCollection();
            services.AddDisputeService();
            Assert.IsTrue(services.Any(x => x.ServiceType == typeof(IDisputeService)));
        }
    }
}
