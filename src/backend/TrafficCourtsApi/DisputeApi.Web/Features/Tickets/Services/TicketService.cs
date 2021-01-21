using DisputeApi.Web.Features.Tickets.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DisputeApi.Web.Features.Tickets.Services
{

    public interface ITicketService
    {
        Task<IQueryable<Ticket>> ListTickets();
    }

    public class TicketService : ITicketService
    {
        private readonly ILogger<TicketService> _logger;

        public TicketService(ILogger<TicketService> logger)
        {
            _logger = logger;
        }
        public async Task<IQueryable<Ticket>> ListTickets()
        {
            _logger.LogInformation("Returning list of mock tickets");
            return await Task.FromResult(new List<Ticket> { new Ticket { Id = "BCT111111111", Description = "Traffic Violation 1" }, new Ticket { Id = "BCT222222222", Description = "Traffic Violation 1" } }.AsQueryable());
        }
    }
}
