using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DisputeApi.Web.Features.Tickets;
using DisputeApi.Web.Features.Tickets.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace DisputeApi.Web.Test.Features.Tickets.Configuration
{
    [ExcludeFromCodeCoverage]
    public class TicketServiceConfigurationExtensionTest
    {
        [Test]
        public void should_register_necessary_service()
        {
            var services = new ServiceCollection();
            services.AddTicketService();
            Assert.IsTrue(services.Any(x => x.ServiceType == typeof(ITicketsService)));
        }
    }
}
