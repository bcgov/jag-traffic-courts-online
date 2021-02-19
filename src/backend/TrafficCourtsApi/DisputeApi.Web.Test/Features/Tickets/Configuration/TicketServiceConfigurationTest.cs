using DisputeApi.Web.Features.TicketService.Configuration;
using DisputeApi.Web.Features.TicketService;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Linq;

namespace DisputeApi.Web.Test.Features.Tickets.Configuration
{
    public class TicketServiceConfigurationExtensionTest
    {
        [Test]
        public void should_register_necessary_service()
        {
            var services = new ServiceCollection();
            services.AddTicketService();
            Assert.IsTrue(services.Any(x => x.ServiceType == typeof(ITicketService)));
        }
    }
}
