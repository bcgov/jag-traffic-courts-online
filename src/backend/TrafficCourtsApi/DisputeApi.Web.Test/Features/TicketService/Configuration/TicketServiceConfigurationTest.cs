using System.Diagnostics.CodeAnalysis;
using DisputeApi.Web.Features.TicketService;
using DisputeApi.Web.Features.TicketService.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Linq;

namespace DisputeApi.Web.Test.Features.TicketService.Configuration
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
