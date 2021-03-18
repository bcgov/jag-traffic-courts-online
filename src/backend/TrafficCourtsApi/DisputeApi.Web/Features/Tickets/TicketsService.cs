using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DisputeApi.Web.Infrastructure;
using DisputeApi.Web.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DisputeApi.Web.Features.Tickets
{

    public interface ITicketsService
    {
        Task<Ticket> SaveTicket(Ticket ticket);
        Task<IEnumerable<Ticket>> GetTickets();
    }

    public class TicketsService : ITicketsService
    {
        private readonly ILogger<TicketsService> _logger;

        private readonly ViolationContext _context;

        public TicketsService(ILogger<TicketsService> logger, ViolationContext context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Ticket> SaveTicket(Ticket ticket)
        {
            _logger.LogDebug("Saving mock ticket");

            await _context.Tickets.AddAsync(ticket);
            await _context.SaveChangesAsync();

            return ticket;
        }

        public async Task<IEnumerable<Ticket>> GetTickets()
        {
            _logger.LogDebug("Returning list of mock tickets");
            var tickets = await _context.Tickets.ToListAsync();

            return tickets;
        }
    }
}
