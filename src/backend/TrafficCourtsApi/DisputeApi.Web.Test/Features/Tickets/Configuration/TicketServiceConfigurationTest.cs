using DisputeApi.Web.Features.Tickets.Configuration;
using DisputeApi.Web.Features.Tickets.Services;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Linq;

namespace DisputeApi.Web.Test.Features.Tickets.Configuration
{
    public class TicketServiceConfigurationTest
    {
        [Test]
        public void should_register_necessary_service()
        {
            var services = new ServiceCollection();
            TicketServiceConfiguration.AddTicketService(services);
            Assert.IsTrue(services.Any(x => x.ServiceType == typeof(ITicketService)));
        }
    }
}
