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
#pragma warning disable IDE1006 // Naming Styles
        public void should_register_necessary_service()
#pragma warning restore IDE1006 // Naming Styles
        {
            var services = new ServiceCollection();
            services.AddTicketService();
            Assert.Contains(services, x => x.ServiceType == typeof(ITicketsService));
        }
    }
}
