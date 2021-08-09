using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Gov.CitizenApi.Features.Tickets;
using Gov.CitizenApi.Features.Tickets.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Gov.CitizenApi.Test.Features.Tickets.Configuration
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
