
using DisputeApi.Web.Features.TicketService.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DisputeApi.Web.Features.TicketService.DBContexts;

namespace DisputeApi.Web.Features.TicketService
{

    public interface ITicketService
    {
        Task<string> SaveTicket(Ticket ticket);
        Task<IQueryable<Ticket>> GetTickets();
    }

    public class TicketService : ITicketService
    {
        private readonly ILogger<TicketService> _logger;

        private readonly TicketContext _context;

        public TicketService(ILogger<TicketService> logger, TicketContext context)
        {
            _logger = logger;
            _context = context;
            if (!_context.Tickets.Any())
            {
                _context.Tickets.Add(new Ticket
                {
                    TicketNumber = 11234,
                    Name = "John Doe",
                    DateOfIssue = "11-12-2002",
                    TimeOfIssue = "12:23",
                    DriversLicence = "L2323G7"
                });
                _context.SaveChanges();
            }
        }
        public async Task<string> SaveTicket(Ticket ticket)
        {
            _logger.LogInformation("Returning list of mock tickets");
            // return await Task.FromResult(new List<Ticket> { new Ticket { Id = "BCT111111111", Description = "Traffic Violation 1" }, new Ticket { Id = "BCT222222222", Description = "Traffic Violation 1" } }.AsQueryable());
            _context.Tickets.Add(ticket);
            _context.SaveChanges();
            return "Success";
        }

        public async Task<IQueryable<Ticket>> GetTickets()
        {
            _logger.LogInformation("Returning list of mock tickets");
            return _context.Tickets;
        }
    }
}
