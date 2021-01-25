using DisputeApi.Web.Features.Tickets.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DisputeApi.Web.Features.Tickets.Configuration
{
    public static class TicketServiceConfigurationExtension
    {
        public static void AddTicketService(this IServiceCollection collection)
        {
            collection.AddTransient<ITicketService, TicketService>();
        }
    }
}
