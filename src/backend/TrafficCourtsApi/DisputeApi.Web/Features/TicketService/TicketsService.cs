﻿
using DisputeApi.Web.Features.TicketService.Models;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using DisputeApi.Web.Features.TicketService.DBContexts;

namespace DisputeApi.Web.Features.TicketService.Service
{

    public interface ITicketsService
    {
        Task<Ticket> SaveTicket(Ticket ticket);
        Task<IQueryable<Ticket>> GetTickets();
    }

    public class TicketsService : ITicketsService
    {
        private readonly ILogger<TicketsService> _logger;

        private readonly TicketContext _context;

        public TicketsService(ILogger<TicketsService> logger, TicketContext context)
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
        public async Task<Ticket> SaveTicket(Ticket ticket)
        {
            _logger.LogInformation("Saving mock ticket");
            _context.Tickets.Add(ticket);
            _context.SaveChanges();
            return await Task.FromResult(ticket);
        }

        public async Task<IQueryable<Ticket>> GetTickets()
        {
            _logger.LogInformation("Returning list of mock tickets");
            return await Task.FromResult(_context.Tickets);
        }
    }
}
