using DisputeApi.Web.Features.Tickets.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DisputeApi.Web.Features.Tickets.Configuration
{
    public class TicketServiceConfiguration
    {
        public static void AddTicketService(IServiceCollection collection)
        {
            collection.AddTransient<ITicketService, TicketService>();
        }
    }
}
