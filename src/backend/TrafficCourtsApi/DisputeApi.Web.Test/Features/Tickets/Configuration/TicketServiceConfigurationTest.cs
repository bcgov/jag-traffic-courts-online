using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DisputeApi.Web.Features.Tickets;
using DisputeApi.Web.Features.Tickets.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DisputeApi.Web.Test.Features.Tickets.Configuration
{
    [ExcludeFromCodeCoverage]
    public class TicketServiceConfigurationExtensionTest
    {
        [Fact]
        public void should_register_necessary_service()
        {
            var services = new ServiceCollection();
            services.AddTicketService();
            Assert.Contains(services, x => x.ServiceType == typeof(ITicketsService));
        }
    }
}
