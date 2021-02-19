
using DisputeApi.Web.Features.TicketService.Service;
using Microsoft.Extensions.DependencyInjection;

namespace DisputeApi.Web.Features.TicketService.Configuration
{
    /// <summary>
    /// Extension to inject ticket configuration service in service collection
    /// </summary>
    public static class TicketServiceConfigurationExtension
    {
        public static void AddTicketService(this IServiceCollection collection)
        {
            collection.AddTransient<ITicketsService, TicketsService>();
        }
    }
}
