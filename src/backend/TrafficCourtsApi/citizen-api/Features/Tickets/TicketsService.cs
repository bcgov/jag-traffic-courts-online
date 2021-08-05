using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Gov.CitizenApi.Features.Tickets.DBModel;
using Gov.CitizenApi.Infrastructure;
using Gov.CitizenApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Gov.CitizenApi.Features.Tickets
{

    public interface ITicketsService
    {
        Task<Ticket> CreateTicketAsync(Ticket ticket);
        Task<IEnumerable<Ticket>> GetTickets();
        Task<Ticket> FindTicketAsync(string ticketNumber);
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

        public async Task<Ticket> CreateTicketAsync(Ticket ticket)
        {
            var existedTicket = await FindTicketAsync(ticket.ViolationTicketNumber);
            if (existedTicket == null)
            {
                _logger.LogDebug("Creating ticket");
                var createdTicket = await _context.Tickets.AddAsync(ticket);
                await _context.SaveChangesAsync();
                return createdTicket.Entity;

            }
            _logger.LogError("found the ticket for the same ticketNumber={ticketNumber}", ticket.ViolationTicketNumber);
            return new Ticket { Id = 0 };
        }


        public async Task<IEnumerable<Ticket>> GetTickets()
        {
            _logger.LogDebug("Returning list of mock tickets");
            var tickets = await _context.Tickets.ToListAsync();

            return tickets;
        }

        public async Task<Ticket> FindTicketAsync(string ticketNumber)
        {
            _logger.LogDebug("Find ticket for ticketNumber {ticketNumber}", ticketNumber);

            var ticket = await _context.Tickets.FirstOrDefaultAsync(_ => _.ViolationTicketNumber == ticketNumber);
            if (ticket != null)
            {
                ticket.Offences = new Collection<DBModel.Offence>(
                    _context.Offences.Where(m => m.TicketId == ticket.Id).ToList()
                    );
            }
            return ticket;
        }
    }
}
